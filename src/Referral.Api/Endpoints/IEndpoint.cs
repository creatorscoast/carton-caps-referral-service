namespace Referral.Api.Endpoints;

public interface IEndpoint
{
    static abstract void DefineEndpoints(IEndpointRouteBuilder app);
    static abstract void AddServices(IServiceCollection services, IConfiguration configuration);
}
