using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Common.Interfaces;
using Product.Infrastructure.Services;
using Product.Infrastructure.Settings;

namespace Product.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Cloudinary Settings
        services.Configure<CloudinarySettings>(
            configuration.GetSection(CloudinarySettings.SectionName)
        );

        // Cloudinary Service
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }
}
