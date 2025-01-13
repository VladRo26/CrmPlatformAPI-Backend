﻿using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using CrmPlatformAPI.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace CrmPlatformAPI.Extensions
{
    public static class  ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photo Upload API", Version = "v1" });
                c.OperationFilter<FileUploadOperationFilter>(); // Add custom operation filter for file uploads
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CRMConnectionString")).EnableSensitiveDataLogging();
            });

            services.AddHttpClient("LLMClient", client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:8000"); // URL of FastAPI backend
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            services.AddCors();
            services.AddScoped<IRepositoryBeneficiaryCompany, RepositoryBeneficiaryCompany>();
            services.AddScoped<IRepositorySoftwareCompany, RepositorySoftwareCompany>();
            services.AddScoped<IRepositoryHomeImage, RepositoryHomeImage>();
            services.AddScoped<IRepositoryContract, RepositoryContract>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRepositoryUser, RepositoryUser>();
            services.AddScoped<IRepositoryCompanyPhoto, RepositoryCompanyPhoto>();
            services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<IRepositoryLLM, RepositoryLLM>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
 
            return services;

        }   
    }
}
