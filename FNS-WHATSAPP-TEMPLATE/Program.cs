using Core.AutoMapperProfile;
using Core.Interfaces.Repository;
using Core.Interfaces.UnitOfWork;
using Core.Repository;
using Core.UnitOfWork;
using Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;

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

        services.AddScoped<IFamilyRepository, FamilyRepository>();

        services.AddSingleton<IWeddingDbUow, WeddingDbUow>();
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
