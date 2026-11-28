using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.Api.Data;

namespace University.Api.Common;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("University");

        // The build-time OpenAPI generator runs this code without any configuration.
        if (string.IsNullOrWhiteSpace(connectionString) && !IsOpenApiGenerator())
        {
            throw new InvalidOperationException(
                "Connection string 'University' is not configured. Set ConnectionStrings:University " +
                "in user secrets (development) or ConnectionStrings__University (deployment).");
        }

        services.AddDbContext<UniversityDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        return services;
    }

    public static IServiceCollection AddApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    internal static bool IsOpenApiGenerator() =>
        Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
}
