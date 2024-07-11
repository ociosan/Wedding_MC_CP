using Azure.Interfaces.Repository;
using Azure.Repository;
using Core.Interfaces.UnitOfWork;
using Core.UnitOfWork;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Core.Helpers;
using Core.Interfaces.Helper;
using Core.AutoMapperProfile;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAzureClients(azureClientFactoryBuilder => {
            azureClientFactoryBuilder.AddSecretClient(new Uri(Environment.GetEnvironmentVariable("KeyVault")));
            azureClientFactoryBuilder.AddBlobServiceClient(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
        });

        services.AddSingleton<IKeyVaultRepository, KeyVaultRepository>();
        services.AddSingleton<IStorageAccountRepository, StorageAccountRepository>();
        services.AddSingleton<IServiceBusRepository, ServiceBusRepository>();

        services.AddSingleton<IAzureUow, AzureUow>();
        services.AddSingleton<IHelpersUow, HelpersUow>();

        services.AddScoped<IPdfHelper, PdfHelper>();
        services.AddScoped<IDapperDbHelper, DapperDbHelper>();


        services.AddSingleton<IWeddingDbUow, WeddingDbUow>();
        services.AddSingleton<IAzureUow, AzureUow>();
        services.AddAutoMapper(typeof(FamilyProfile).Assembly);

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
