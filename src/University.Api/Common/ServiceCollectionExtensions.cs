namespace University.Api.Common;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
}
