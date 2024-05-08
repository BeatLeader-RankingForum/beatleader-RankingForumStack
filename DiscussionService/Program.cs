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
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rankingforum-mq";
var rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";

var rabbitMqUri = new Uri($"amqp://{rabbitMqHost}:{rabbitMqPort}");

builder.Services.AddMassTransit(BusFactoryConfigurator =>
{
    BusFactoryConfigurator.SetKebabCaseEndpointNameFormatter();

    BusFactoryConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(rabbitMqUri, h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? builder.Configuration["MessageBroker:Username"] ?? "guest");
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? builder.Configuration["MessageBroker:Password"] ?? "guest");
        });

        configurator.ConfigureEndpoints(context);
    });
});

Console.WriteLine($"Connecting to RabbitMQ at {$"amqp://{rabbitMqHost}:{rabbitMqPort}"}");

var app = builder.Build();

AppDbContext.ApplyMigrations(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();