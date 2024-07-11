using Azure.Interfaces.Repository;
using Azure.Repository;
using Core.AutoMapperProfile;
using Core.Helpers;
using Core.Interfaces.Helper;
using Core.Interfaces.UnitOfWork;
using Core.UnitOfWork;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(worker => worker.UseNewtonsoftJson())
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

        services.AddSingleton<IWeddingDbUow, WeddingDbUow>();
        services.AddSingleton<IAzureUow, AzureUow>();
        services.AddSingleton<IDapperDbHelper, DapperDbHelper>();

        services.AddAutoMapper(typeof(FamilyProfile).Assembly);
    })
    .Build();

host.Run();
