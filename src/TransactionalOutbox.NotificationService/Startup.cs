using TransactionalOutbox.NotificationService.BackgroundServices;
using TransactionalOutbox.NotificationService.Database;
using TransactionalOutbox.NotificationService.Kafka;
using TransactionalOutbox.NotificationService.Services;

namespace TransactionalOutbox.NotificationService;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpcReflection();
        services.AddGrpc();
        services.AddDatabase(_configuration);
        services.AddBackgroundServices();
        services.AddKafka(_configuration);
        services.AddServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<Grpc.NotificationService>();
            endpoints.MapGrpcReflectionService();
        });
    }
}