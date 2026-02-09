using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Common.Interfaces;
using Product.Persistence.Repositories;

namespace Product.Persistence;

/// <summary>
/// Persistence layer dependency injection configuration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers persistence layer services including DbContext and repositories.
    /// </summary>
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

        // Product Repository registration - single instance serves all interfaces (ISP)
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository>(sp => sp.GetRequiredService<ProductRepository>());
        services.AddScoped<IDishRepository>(sp => sp.GetRequiredService<ProductRepository>());
        services.AddScoped<IDishCategoryRepository>(sp => sp.GetRequiredService<ProductRepository>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProductRepository>());

        // Order Repository registration
        services.AddScoped<IOrderRepository, OrderRepository>();

        // TableSession Repository registration
        services.AddScoped<ITableSessionRepository, TableSessionRepository>();

        // Reservation Repository registration
        services.AddScoped<IReservationRepository, ReservationRepository>();

        return services;
    }
}
