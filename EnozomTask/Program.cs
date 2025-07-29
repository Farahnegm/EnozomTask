
using EnozomTask.InfraStructure.Clockify;
using EnozomTask.InfraStructure.Extensions;
using Restaurants.Api.Extensions;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Seeders;

namespace EnozomTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.AddPresentationServices();
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);
           


            var app = builder.Build();

                
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                seeder.SeedAsync().GetAwaiter().GetResult();
            }

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
