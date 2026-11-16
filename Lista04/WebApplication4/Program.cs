using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog();

builder.Configuration.AddJsonFile("extrasettings.json");
builder.Configuration.AddXmlFile("extrasettings.xml");
builder.Services.Configure<Labs>(builder.Configuration.GetSection("labs"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/zadanie1", (IConfiguration config, IOptions<Labs> labs) =>  {
    // Punkt 2
    var foo = config["foo"];
    var exercise = config["labs:exercise"];
    var subject = config["labs:subject"];

    // Punkt 3
    var secret = config.GetSection("secret");
    var user = secret["user"];
    var password = secret.GetValue<string>("password");

    // Punkt 4
    var subjectLabs = labs.Value.Subject;

    return new {
        foo, exercise, subject, user, password, labs = labs.Value
    };
});

app.MapGet("/zadanie2", (ILoggerFactory loggerFactory, ILogger<Program> logger, IConfiguration config) =>
{
    var factoryLogger = loggerFactory.CreateLogger("Test1");
    factoryLogger.LogInformation("Log Information z ILoggerFactory!");
    logger.LogInformation("Log Information z ILogger!");

    return "Zapisano logi!";
});

app.Run();

public class Labs
{
    public int Exercise { get; set; }
    public int List { get; set; }
    public string Subject { get; set; } = "";
};