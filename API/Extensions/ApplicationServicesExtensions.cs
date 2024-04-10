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

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Use this when using AzureKeyVault
            /*services.AddAzureClients(azureClientFactoryBuilder => {
                azureClientFactoryBuilder.AddSecretClient(config.GetSection("KeyVault"));

            });*/
            services.AddDbContext<WeddingDBContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultDBConnection"));
            });

            services.AddScoped<IFamilyRepository, FamilyRepository>();
            services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.AddScoped<IConfirmAssistanceService, ConfirmAssistanceService>();


            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ErrorHandlingMiddleware>();
            services.AddAutoMapper(typeof(FamilyProfile).Assembly);
            services.AddAutoMapper(typeof(FamilyMemberProfile).Assembly);



            return services;
        }
    }
}
