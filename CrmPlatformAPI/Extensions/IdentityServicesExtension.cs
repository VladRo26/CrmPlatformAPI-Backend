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
                });

            return services;
        }
    }
}
