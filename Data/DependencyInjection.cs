using Data.POCOs;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBaseRepository<UserPoco>, UserRepository>();
        services.AddSingleton<IBaseRepository<AccountPoco>, AccountRepository>();

        return services;
    }
}