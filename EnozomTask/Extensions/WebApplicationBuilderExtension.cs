using Microsoft.OpenApi.Models;

namespace Restaurants.Api.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static void AddPresentationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
           


        }
    }
}