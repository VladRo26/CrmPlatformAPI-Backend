using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using CrmPlatformAPI.SingalR;
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
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM Platform API", Version = "v1" });

                            c.OperationFilter<FileUploadOperationFilter>(); // Optional: Custom filter for file uploads

                            // 🔐 Add JWT Bearer authentication support to Swagger UI
                            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                            {
                                In = ParameterLocation.Header,
                                Description = "Enter 'Bearer {your JWT token}'",
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey,
                                Scheme = "Bearer"
                            });

                            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });

                // 📄 Include XML comments (for endpoint docs)
                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CRMConnectionString")).EnableSensitiveDataLogging();
            });

            services.AddHttpClient("LLMClient", client =>
            {
                // Read the LLM service BaseUrl from configuration
                var llmBaseUrl = configuration["LLMService:BaseUrl"];
                client.BaseAddress = new Uri(llmBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("SentimentClient", client =>
            {
                // Read the Sentiment service BaseUrl from configuration
                var sentimentBaseUrl = configuration["SentimentService:BaseUrl"];
                client.BaseAddress = new Uri(sentimentBaseUrl);
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
            services.AddScoped<IRepositoryTicketStatusHistory, RepositoryTicketStatusHistory>();
            services.AddScoped<IRepositoryTicket, RepositoryTicket>();
            services.AddScoped<IRepositoryLLM, RepositoryLLM>();
            services.AddScoped<IRepositorySentimentAnalysis, RepositorySentimentAnalysis>();
            services.AddScoped<IRepositoryFeedbackSentiment, RepositoryFeedbackSentiment>();
            services.AddScoped<IRepositorySentimentAnalysis, RepositorySentimentAnalysis>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRepositoryTicketAttachment, RepositoryTicketAttachment>();
            services.AddScoped<ActivityLog>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();


            return services;

        }   
    }
}
