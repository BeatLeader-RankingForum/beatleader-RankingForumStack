using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.DbContexts;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;
        public UserController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return _userDbContext.Users;
        }

        [HttpGet("{entryId:int}")]
        public async Task<ActionResult<User>> GetById(int entryId)
        {
            var user = await _userDbContext.Users.FindAsync(entryId);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            await _userDbContext.Users.AddAsync(user);
            await _userDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { entryId = user.EntryId }, user);
        }

        [HttpPut("{entryId:int}")]
        public async Task<IActionResult> Update(int entryId, User user)
        {
            if (entryId != user.EntryId)
            {
                return BadRequest();
            }
            _userDbContext.Users.Update(user);
            await _userDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{entryId:int}")]
        public async Task<IActionResult> Delete(int entryId)
        {
            var user = await _userDbContext.Users.FindAsync(entryId);
            if (user == null)
            {
                return NotFound();
            }
            _userDbContext.Users.Remove(user);
            await _userDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
