using AspNetCoreRateLimit;
using Contracts.Auth.OptionsSetup;
using DiscussionService;
using DiscussionService.Logic;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "DiscussionService", Version = "v1" });
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

// DI
builder.Services.AddScoped<MapDiscussionLogic>();
builder.Services.AddScoped<DifficultyDiscussionLogic>();

// EF CORE
string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "rf-discussion";
string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "SuperStrong!";
string connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword};";
builder.Services.AddDbContextPool<DiscussionDbContext>(options => options.UseNpgsql(connectionString));

// MASSTRANSIT
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

// OPTIONS
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.ConfigureOptions<AuthorizeOptionsSetup>();

// AUTHENTICATION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

// RATELIMITING
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(opt =>
{
    opt.EnableEndpointRateLimiting = true;
    opt.StackBlockedRequests = false;
    opt.HttpStatusCode = 429;
    opt.RealIpHeader = "X-Real-IP";
    opt.ClientIdHeader = "X-Client-Id";
    opt.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "10s",
            Limit = 50,
        },
    };

});
// TODO: add more rate limit rules
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins(
                    "http://localhost:8888",
                    "https://localhost:5173",
                    "https://rankingforum.lightai.dev")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

DiscussionDbContext.ApplyMigrations(app);

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

app.UseCors("AllowSpecificOrigins");

if (Environment.GetEnvironmentVariable("LOADTEST") != "true") app.UseIpRateLimiting();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

app.Run();