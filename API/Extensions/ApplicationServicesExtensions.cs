using Azure.Core;
using Azure.Interfaces.Repository;
using Azure.Repository;
using Core;
using Core.AutoMapperProfile;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Helper;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Middleware;
using Core.Repository;
using Core.Services;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAzureClients(azureClientFactoryBuilder => {
                azureClientFactoryBuilder.AddSecretClient(config.GetSection("KeyVault"));
                azureClientFactoryBuilder.AddBlobServiceClient(config.GetSection("StorageAccount"));
            });
            services.AddDbContext<WeddingDBContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultDBConnection"));
            });

            services.AddSingleton<IKeyVaultRepository, KeyVaultRepository>();
            services.AddSingleton<IStorageAccountRepository, StorageAccountRepository>();

            services.AddScoped<IFamilyRepository, FamilyRepository>();
            services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.AddScoped<IPdfHelper, PdfHelper>();
            services.AddScoped<IConfirmAssistanceService, ConfirmAssistanceService>();


            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ErrorHandlingMiddleware>();
            services.AddAutoMapper(typeof(FamilyProfile).Assembly);
            services.AddAutoMapper(typeof(FamilyMemberProfile).Assembly);



            return services;
        }
    }
}
