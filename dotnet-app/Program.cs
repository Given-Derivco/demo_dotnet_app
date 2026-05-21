var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0";
var hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? System.Net.Dns.GetHostName();
var instance = hostname.Length > 12 ? hostname[..12] : hostname;
var started = DateTime.UtcNow;
var readyAfter = double.Parse(Environment.GetEnvironmentVariable("READY_AFTER_SECONDS") ?? "3");
var requests = 0;

app.MapGet("/", () =>
{
    Interlocked.Increment(ref requests);
    return Results.Content(
        $"<h1>ASP.NET Core on Launchpad</h1><p>version <b>{version}</b> · instance <code>{instance}</code> · requests {requests}</p><p><a href=\"/swagger\">OpenAPI / Swagger</a></p>",
        "text/html");
});

app.MapGet("/api/health", () => new { status = "ok" });

app.MapGet("/version", () => new
{
    version,
    instance,
    uptime_s = (int)(DateTime.UtcNow - started).TotalSeconds,
    requests
});

app.MapGet("/healthz", () =>
{
    if ((DateTime.UtcNow - started).TotalSeconds < readyAfter)
        return Results.Json(new { status = "starting" }, statusCode: 503);
    return Results.Json(new { status = "ready" });
});

app.Run();
