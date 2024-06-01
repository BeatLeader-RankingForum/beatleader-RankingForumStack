using CommentService;
using Contracts.Auth.OptionsSetup;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "CommentService", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "rf-comment";
string dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "SuperStrong!";
string connectionString = $"Data Source={dbHost}; Initial Catalog={dbName}; User ID=sa; Password={dbPassword}; Encrypt=true; TrustServerCertificate=true;";
builder.Services.AddDbContext<CommentDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMassTransit(busFactoryConfigurator =>
{
    busFactoryConfigurator.SetKebabCaseEndpointNameFormatter();

    busFactoryConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri($"amqp://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}:{Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"}"), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest");
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest");
        });

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.ConfigureOptions<AuthorizeOptionsSetup>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

CommentDbContext.ApplyMigrations(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction() && Environment.GetEnvironmentVariable("JWT_SECRET_KEY").ToLower().Contains("development"))
{
    Console.WriteLine("The application will be terminated due to an insecure JWT_SECRET_KEY.");
    Environment.FailFast("The application has been terminated due to an insecure JWT_SECRET_KEY.");
}

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();