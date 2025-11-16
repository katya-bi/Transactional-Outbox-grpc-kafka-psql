using TransactionalOutbox.OrderService.BackgroundServices;
using TransactionalOutbox.OrderService.Database;
using TransactionalOutbox.OrderService.Kafka;
using TransactionalOutbox.OrderService.Services;

namespace TransactionalOutbox.OrderService;

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
        services.AddKafka(_configuration);
        services.AddBackgroundServices();
        services.AddServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<Grpc.OrderService>();
            endpoints.MapGrpcReflectionService();
        });
    }
}
