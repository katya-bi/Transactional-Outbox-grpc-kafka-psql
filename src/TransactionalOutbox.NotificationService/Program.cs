using Serilog;
using TransactionalOutbox.NotificationService;

var configuration = BuildConfiguration(args);

ConfigureLogging(configuration);

const string AppName = "NotificationService";
try
{
    Log.Information("Starting up {ApplicationName}", AppName);
    var host = CreateAndConfigureHost(args);
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "{ApplicationName} terminated unexpectedly", AppName);
}
finally
{
    Log.CloseAndFlush();
}

static IConfiguration BuildConfiguration(string[] args)
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();
}

static void ConfigureLogging(IConfiguration configuration)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", AppName)
        .CreateLogger();
}

static IHost CreateAndConfigureHost(string[] args)
{
    var hostBuilder = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureHostOptions(hostOptions =>
        {
            hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(30); // Graceful shutdown
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

    return hostBuilder.Build();
}