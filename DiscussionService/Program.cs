using DiscussionService;
using DiscussionService.Logic;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<MapDiscussionLogic>();

string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "rf-discussion";
string dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "SuperStrong!";
string connectionString = $"Data Source={dbHost}; Initial Catalog={dbName}; User ID=sa; Password={dbPassword}; Encrypt=true; TrustServerCertificate=true;";
builder.Services.AddDbContext<DiscussionDbContext>(options => options.UseSqlServer(connectionString));


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

var app = builder.Build();

DiscussionDbContext.ApplyMigrations(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();