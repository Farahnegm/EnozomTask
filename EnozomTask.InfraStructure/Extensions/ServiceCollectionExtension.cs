
using EnozomTask.Application.Interfaces.Factories;
using EnozomTask.Application.Interfaces.Services;
using EnozomTask.Domain.Interfaces.Strategies;
using EnozomTask.InfraStructure.Services;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.Clockify;
using EnozomTask.InfraStructure.Factories;
using EnozomTask.InfraStructure.persistence;
using EnozomTask.InfraStructure.Repositories;
using EnozomTask.InfraStructure.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EnozomTask.InfraStructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            
            services.AddScoped<IClockifySyncService, ClockifySyncService>();
            services.Configure<ClockifySettings>(configuration.GetSection("Clockify"));
            services.AddScoped<ITimeEntryOrchestrationService, TimeEntryOrchestrationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IEntityFactory, EntityFactory>();
            services.AddScoped<ISyncStrategy, ClockifySyncStrategy>();
            services.AddScoped<IProjectService, ProjectService>();

            services.AddHttpClient();
        }
    }
}