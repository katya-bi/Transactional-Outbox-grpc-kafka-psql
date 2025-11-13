using Dapper;
using Npgsql;
using TransactionalOutbox.OrderService.Database.Connections;
using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Migrations;
using TransactionalOutbox.OrderService.Database.Repositories;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;

namespace TransactionalOutbox.OrderService.Database;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton(_ =>
        {
            var connStr = configuration.GetSection("Database:Postgres")["ConnectionString"]!;

            return NpgsqlDataSource.Create(connStr);
        });

        return services
            .AddScoped<DbConnectionFactory>()
            .AddScoped<IDbConnectionFactory>(x => x.GetRequiredService<DbConnectionFactory>())
            .AddScoped<ITransactionProvider>(x => x.GetRequiredService<DbConnectionFactory>())
            .AddSingleton<IDbMigrator, DbUpMigrator>()
            .AddRepositories();
    }
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IOrderRepository, OrderRepository>();
    }
}