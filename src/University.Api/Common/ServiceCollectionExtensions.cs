using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var otel = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("University.Api"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation());

        // Export only when a collector is configured (the Aspire Dashboard locally). Production
        // runs without one and sends its logs to stdout instead.
        if (!string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            otel.UseOtlpExporter();
        }

        return services;
    }

    internal static bool IsOpenApiGenerator() =>
        Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
}
