using DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Models.Twitter;

namespace DataAccess.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IRepository<MessageData>, TwitterRepository>();
            return services;
        }
    }
}
