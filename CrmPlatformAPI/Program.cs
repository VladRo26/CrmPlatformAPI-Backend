
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CRMConnectionString"));
            });

            builder.Services.AddScoped<IRepositoryBeneficiaryCompanies, RepositoryBeneficiaryCompanies>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
