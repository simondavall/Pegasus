using Microsoft.Extensions.DependencyInjection;

namespace Pegasus.Library.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiEndpoints(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationEndpoint, AuthenticationEndpoint>();
            services.AddTransient<IProjectsEndpoint, ProjectsEndpoint>();
            services.AddTransient<ITasksEndpoint, TasksEndpoint>();
            services.AddTransient<ICommentsEndpoint, CommentsEndpoint>();
            services.AddTransient<IAccountsEndpoint, AccountsEndpoint>();
            services.AddTransient<IManageEndpoint, ManageEndpoint>();

            return services;
        }
    }
}