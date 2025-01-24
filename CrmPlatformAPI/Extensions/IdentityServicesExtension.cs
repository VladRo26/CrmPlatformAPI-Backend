using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CrmPlatformAPI.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<User>(
               options =>
               {
                   options.Password.RequireDigit = false;
               })
                  .AddRoles<Role>()
                   .AddRoleManager<RoleManager<Role>>()
                  .AddEntityFrameworkStores<ApplicationDbContext>();


            services.Configure<IdentityOptions>(IdentityOptions =>
            {
                IdentityOptions.Password.RequireDigit = false;
                IdentityOptions.Password.RequireLowercase = false;
                IdentityOptions.Password.RequireNonAlphanumeric = false;
                IdentityOptions.Password.RequireUppercase = false;
                IdentityOptions.Password.RequiredLength = 4;
                IdentityOptions.User.RequireUniqueEmail = false;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var token = config["TokenKey"] ?? throw new Exception("Cannot find token key in appsettings.json");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorizationBuilder()
                .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                .AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Moderator", "Admin"))
                .AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"))
                .AddPolicy("RequireDefaultRole", policy => policy.RequireRole("Default", "Admin", "Moderator"));


            return services;
        }
    }
}
