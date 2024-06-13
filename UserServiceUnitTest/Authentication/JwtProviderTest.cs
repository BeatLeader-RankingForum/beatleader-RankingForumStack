using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contracts.Auth;
using Contracts.Auth.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using UserService;
using UserService.Authentication;
using UserService.Models;

namespace UserServiceUnitTest.Authentication;

[TestFixture]
[TestOf(typeof(JwtProvider))]
public class JwtProviderTest
{

    [TestFixture]
    public class JwtTokenGeneratorTests
    {
        private Mock<IOptions<JwtOptions>> _mockJwtOptions;
        private Mock<IOptions<JwtBearerOptions>> _mockJwtBearerOptions;
        private Mock<UserDbContext> _mockDbContext;
        private JwtProvider _jwtProvider;

        [SetUp]
        public void SetUp()
        {
            // set up JwtOptions
            JwtOptions jwtOptions = new JwtOptions()
            {
                Issuer = "UserService",
                Audience = "RankingForumStack",
                SecretKey = "SecretKeyTesting-543215836574337",
                JwtExpiryInMinutes = 30,
                RefreshExpiryInDays = 7
            };
            _mockJwtOptions = new();
            _mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);
            
            // set up JwtBearerOptions
            JwtBearerOptions jwtBearerOptions = new JwtBearerOptions()
            {
                TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    NameClaimType = JwtRegisteredClaimNames.Sub
                }
            };
            _mockJwtBearerOptions = new();
            _mockJwtBearerOptions.Setup(x => x.Value).Returns(jwtBearerOptions);
            
            // set up dbcontext
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockDbContext = new Mock<UserDbContext>(options);
            
            // set up JwtProvider
            _jwtProvider = new JwtProvider(
                options: _mockJwtOptions.Object,
                dbContext: _mockDbContext.Object,
                jwtBearerOptions: _mockJwtBearerOptions.Object);
        }

        [Test]
        public void GenerateJwtToken_ShouldReturnValidToken()
        {
            // Arrange
            var userId = "6533437789066";
            var roles = new List<Role>
            {
                Role.User,
                Role.Moderator
            };
            var user = new User
            {
                Id = userId,
                Roles = roles
            };

            // Act
            var token = _jwtProvider.GenerateJwtToken(user);

            // Assert
            Assert.IsNotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = _mockJwtBearerOptions.Object.Value.TokenValidationParameters.Clone();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            List<Role> rolesFromClaim = roleClaims.Select(c => Enum.Parse<Role>(c.Value)).ToList();

            Assert.That(userIdClaim?.Value, Is.EqualTo(user.Id));
            Assert.True(user.Roles.SequenceEqual(rolesFromClaim));
        }
        
        [Test]
        public void GenerateJwtToken_ShouldReturnValidToken_ForEmptyRoles()
        {
            // Arrange
            var userId = "6533437789066";
            var roles = new List<Role>
            {
            };
            var user = new User
            {
                Id = userId,
                Roles = roles
            };

            // Act
            var token = _jwtProvider.GenerateJwtToken(user);

            // Assert
            Assert.IsNotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = _mockJwtBearerOptions.Object.Value.TokenValidationParameters.Clone();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            List<Role> rolesFromClaim = roleClaims.Select(c => Enum.Parse<Role>(c.Value)).ToList();

            Assert.That(userIdClaim?.Value, Is.EqualTo(user.Id));
            Assert.True(user.Roles.SequenceEqual(rolesFromClaim));
        }
        
        [Test]
        public void GenerateJwtToken_ShouldReturnArgumentException()
        {
            // Arrange
            var userId = "";
            var roles = new List<Role>
            {
                Role.User
            };
            var user = new User
            {
                Id = userId,
                Roles = roles
            };

            // Act
            try
            {
                _jwtProvider.GenerateJwtToken(user);
            }
            catch (Exception e)
            {
                // Assert
                if (e is ArgumentException)
                {
                    Assert.Pass("Correct exception was thrown.");
                    return;
                }
                Assert.Fail("Wrong exception was thrown.");
            }
            
            Assert.Fail("ArgumentException was not thrown.");
        }
    }
}