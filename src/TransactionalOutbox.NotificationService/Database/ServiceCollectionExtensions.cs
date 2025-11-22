using Dapper;
using Npgsql;
using TransactionalOutbox.NotificationService.Database.Connections;
using TransactionalOutbox.NotificationService.Database.Interfaces;
using TransactionalOutbox.NotificationService.Database.Migrations;
using TransactionalOutbox.NotificationService.Database.Repositories;
using TransactionalOutbox.NotificationService.Database.Repositories.Abstract;

namespace TransactionalOutbox.NotificationService.Database;

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
            .AddScoped<INotificationRepository, NotificationRepository>();
    }
}