using Microsoft.OpenApi.Models;

namespace Restaurants.Api.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static void AddPresentationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
        }
    }
}