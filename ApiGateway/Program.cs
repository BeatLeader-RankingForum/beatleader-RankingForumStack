using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.SetBasePath("/app/config")
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}
else
{
    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}

builder.Services.AddOcelot(builder.Configuration);

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

app.UseCors("AllowSpecificOrigins");

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
