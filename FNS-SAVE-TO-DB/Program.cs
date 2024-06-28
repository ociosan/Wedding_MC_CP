using Azure.Interfaces.Repository;
using Azure.Repository;
using Core.Interfaces.UnitOfWork;
using Core.UnitOfWork;
using Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<WeddingDBContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("DbConnectionString"));
        });

        services.AddAzureClients(azureClientFactoryBuilder => {
            azureClientFactoryBuilder.AddSecretClient(new Uri(Environment.GetEnvironmentVariable("KeyVault")));
            azureClientFactoryBuilder.AddBlobServiceClient(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
        });

        services.AddSingleton<IKeyVaultRepository, KeyVaultRepository>();
        services.AddSingleton<IStorageAccountRepository, StorageAccountRepository>();
        services.AddSingleton<IServiceBusRepository, ServiceBusRepository>();

        services.AddSingleton<IWeddingDbUow, WeddingDbUow>();
        services.AddSingleton<IAzureUow, AzureUow>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Worker", LogEventLevel.Warning)
            .MinimumLevel.Override("Host", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Error)
            .MinimumLevel.Override("Function", LogEventLevel.Error)
            .MinimumLevel.Override("Azure.Storage.Blobs", LogEventLevel.Error)
            .MinimumLevel.Override("Azure.Core", LogEventLevel.Error)
            .WriteTo.MSSqlServer(
                connectionString: Environment.GetEnvironmentVariable("DbConnectionString"),
                sinkOptions: new MSSqlServerSinkOptions()
                {
                    SchemaName = "dbo",
                    TableName = "Log",
                    AutoCreateSqlTable = true

                },
                restrictedToMinimumLevel: (LogEventLevel)2
            ).CreateLogger();

        services.AddSingleton<ILogger>(Log.Logger);
        

     })
    .Build();

host.Run();
