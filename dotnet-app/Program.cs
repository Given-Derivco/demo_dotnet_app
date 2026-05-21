var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "ASP.NET Core demo running on Launchpad");
app.MapGet("/api/health", () => new { status = "ok" });

app.Run();
