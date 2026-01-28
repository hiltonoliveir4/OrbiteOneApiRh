using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
        services.AddScoped<IAfastamentoRepository, AfastamentoRepository>();

        return services;
    }
}
