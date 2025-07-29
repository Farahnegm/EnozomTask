
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.Clockify;
using EnozomTask.InfraStructure.persistence;
using EnozomTask.InfraStructure.Repositories;
using EnozomTask.InfraStructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Infrastructure.Seeders;

namespace EnozomTask.InfraStructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<IClockifyService, ClockifyService>();
            services.Configure<ClockifySettings>(configuration.GetSection("Clockify"));
            services.AddHttpClient();
        }
    }
}