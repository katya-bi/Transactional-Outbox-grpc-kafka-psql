using System.Reflection;
using System.Text.RegularExpressions;
using DbUp;
using TransactionalOutbox.NotificationService.Database.Interfaces;

namespace TransactionalOutbox.NotificationService.Database.Migrations;

internal class DbUpMigrator : IDbMigrator
{
    private readonly string _connectionString;
    private readonly string _migrationsPrefix;
    private readonly Regex _migrationFilterRegex;
    private readonly Assembly _scriptsAssembly;
    private readonly ILogger<DbUpMigrator> _logger;

    public DbUpMigrator(ILogger<DbUpMigrator> logger, IConfiguration configuration)
    {
        _logger = logger;

        _connectionString = configuration.GetSection("Database:Postgres")["ConnectionString"]!;

        _scriptsAssembly = Assembly.GetExecutingAssembly();

        var migrationsSection = configuration.GetSection("Database:Migrations");
        var assemblyName = _scriptsAssembly.GetName().Name;
        var path = migrationsSection["Path"]
            ?? throw new InvalidOperationException("Database:Migrations:Path is missing in configuration");
        _migrationsPrefix = $"{assemblyName}.{path}";

        var pattern = migrationsSection["FilterRegex"]
            ?? throw new InvalidOperationException("Database:Migrations:FilterRegex is missing in configuration");
        _migrationFilterRegex = new Regex(pattern, RegexOptions.IgnoreCase);
    }

    public void Migrate()
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(_scriptsAssembly, scriptName =>
            {
                if (!scriptName.StartsWith(_migrationsPrefix, StringComparison.OrdinalIgnoreCase))
                    return false;
                
                var fileName = scriptName.Substring(_migrationsPrefix.Length + 1); // extra dot

                return _migrationFilterRegex.IsMatch(fileName);
            })
            .LogTo(_logger)
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            throw new ApplicationException("Database migration failed", result.Error);
        }

        _logger.LogInformation("Database migrations applied successfully");
    }
}