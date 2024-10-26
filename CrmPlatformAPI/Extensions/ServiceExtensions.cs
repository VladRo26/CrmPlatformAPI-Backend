﻿using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Extensions
{
    public static class  ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CRMConnectionString")).EnableSensitiveDataLogging();
            });

            services.AddCors();
            services.AddScoped<IRepositoryBeneficiaryCompany, RepositoryBeneficiaryCompany>();
            services.AddScoped<IRepositorySoftwareCompany, RepositorySoftwareCompany>();
            services.AddScoped<IRepositoryHomeImage, RepositoryHomeImage>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            return services;

        }   
    }
}
