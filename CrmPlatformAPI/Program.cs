
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Middleware;
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

           builder.Services.AddIdentityServices(builder.Configuration);




            var app = builder.Build();
            //custom exceptions middleware

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader().WithOrigins("http://localhost:4200", "https://localhost:4200")); // am adaugat cors pentru a putea face requesturi din frontend

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
