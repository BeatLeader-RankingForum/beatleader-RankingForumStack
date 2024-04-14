using Microsoft.EntityFrameworkCore;
using System.Net;
using UserWebApi.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// database context dependency injection
string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "rf-user";
string dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "SuperStrong!";
string connectionString = $"Data Source={dbHost}; Initial Catalog={dbName}; User ID=sa; Password={dbPassword}; Encrypt=true; TrustServerCertificate=true;";
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));
// ===================================

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
