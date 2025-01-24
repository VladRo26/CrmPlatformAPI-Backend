
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Middleware;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using CrmPlatformAPI.SingalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CrmPlatformAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            
            builder.Services.AddServices(builder.Configuration);
            //here i included all the services from the extension class

           builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });


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
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200", "https://localhost:4200")); // am adaugat cors pentru a putea face requesturi din frontend


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();
            app.MapHub<PresenceHub>("hubs/presence");

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();
                await context.Database.MigrateAsync();
                await Seed.SeedSoftwareComp(context);
                await Seed.SeedBeneficiaryComp(context);
                await Seed.SeedContract(context);
                await Seed.SeedTickets(context);
                await Seed.SeedTicketHistory(context);
                await Seed.SeedFeedback(context);


            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            app.Run();
        }
    }
}
