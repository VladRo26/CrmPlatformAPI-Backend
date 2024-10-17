
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CrmPlatformAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            builder.Services.AddServices(builder.Configuration);
            //here i included all the services from the extension class

            builder.Services.AddIdentityCore<User>(
                options =>
                {
                    options.Password.RequireDigit = false;
                })
                   .AddRoles<Role>()
                    .AddRoleManager<RoleManager<Role>>()
                   .AddEntityFrameworkStores<ApplicationDbContext>();
                  

            builder.Services.Configure<IdentityOptions>(IdentityOptions =>
            {
                IdentityOptions.Password.RequireDigit = false;
                IdentityOptions.Password.RequireLowercase = false;
                IdentityOptions.Password.RequireNonAlphanumeric = false;
                IdentityOptions.Password.RequireUppercase = false;
                IdentityOptions.Password.RequiredLength = 4;
                IdentityOptions.User.RequireUniqueEmail = false;
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    };
                });


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseCors(builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader().WithOrigins("http://localhost:4200", "https://localhost:4200")); // am adaugat cors pentru a putea face requesturi din frontend

            app.MapControllers();

            app.Run();
        }
    }
}
