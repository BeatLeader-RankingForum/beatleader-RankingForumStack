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
    options.AddPolicy("AllowDevOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:8888")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            builder.WithOrigins("https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    options.AddPolicy("AllowProdOrigin",
        builder =>
        {
            builder.WithOrigins("https://rankingforum.lightai.dev")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowDevOrigin");

app.UseCors("AllowProdOrigin");

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
