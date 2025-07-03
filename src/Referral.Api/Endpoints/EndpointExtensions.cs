using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Referral.Api.Endpoints;

public static class EndpointExtensions
{
    public static void AddEndpoints(this IServiceCollection services,
        Type type, IConfiguration configuration)
    {
        var endpoints = GetEndpointTypesFromAssembly(type);

        foreach (var endpoint in endpoints)
        {
            endpoint.GetMethod(nameof(IEndpoint.AddServices))!
                .Invoke(null, new object[] { services, configuration });
        }
    }

    public static void AddEndpoints<T>(this IServiceCollection services, IConfiguration configuration)=>
        services.AddEndpoints(typeof(T), configuration);

    public static void UseEndpoints<T>(this IApplicationBuilder app)=>
        app.UseEndpoints(typeof(T));

    public static void UseEndpoints(this IApplicationBuilder app, Type type)
    {
        var endpoints = GetEndpointTypesFromAssembly(type);

        foreach (var endpoint in endpoints)
        {
            endpoint.GetMethod(nameof(IEndpoint.DefineEndpoints))!
                .Invoke(null, new object[] { app });
        }
    }

    private static IEnumerable<TypeInfo> GetEndpointTypesFromAssembly(Type type)
    {
        var endpoints = type.Assembly.DefinedTypes.Where(w => !w.IsAbstract && !w.IsInterface &&
            typeof(IEndpoint).IsAssignableFrom(w));

        return endpoints;
    }
}
