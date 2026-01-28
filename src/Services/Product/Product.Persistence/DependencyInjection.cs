using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Common.Interfaces;
using Product.Persistence.Repositories;

namespace Product.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ProductDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName)
            ));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
