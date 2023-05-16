using BusinessLogic.Clients;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Configuration;

namespace BusinessLogic.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogicDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITwitterClient, TwitterClient>();
            services.AddScoped<ITwitterService, TwitterService>();
            services.Configure<TwitterApiConfiguration>(configuration.GetSection("Twitter"));
            services.AddHostedService<TwitterBackgroundService>();
            return services;
        }
    }
}
