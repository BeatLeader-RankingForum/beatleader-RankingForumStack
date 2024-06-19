using System.Security.Cryptography.X509Certificates;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.SetBasePath("/app/config")
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.WebHost.ConfigureKestrel(so =>
    {
        so.ConfigureHttpsDefaults(options =>
        {
            var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
            var keyPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__KeyPath");

            if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(keyPath))
            {
                throw new InvalidOperationException("Certificate path or key path is not set.");
            }

            options.ServerCertificate = new X509Certificate2(
                Path.Combine(certPath),
                Path.Combine(keyPath)
            );
        });
    });
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
});

var app = builder.Build();

app.UseCors("AllowDevOrigin");

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
