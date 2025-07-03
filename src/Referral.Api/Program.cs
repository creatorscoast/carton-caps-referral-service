
using FluentValidation;
using Referral.Api.Auth;
using Referral.Api.Data;
using Referral.Api.Data.Impl;
using Referral.Api.Endpoints;

namespace Referral.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AnyOrigin", x => x.AllowAnyOrigin());
        });

        builder.Services.AddAuthentication(Constants.ApiKeySchemeName)
            .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(Constants.ApiKeySchemeName, _ => { });
        builder.Services.AddAuthorization();   
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
            new SqlLiteConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnectionString")));
        builder.Services.AddSingleton<DatabaseInitializer>();

        builder.Services.AddEndpoints<Program>(builder.Configuration);
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var app = builder.Build();

        app.UseCors();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();

        app.UseEndpoints<Program>();

        // Default to swagger for testing/mocking/etc
        app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
            .ExcludeFromDescription();

        var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
        await databaseInitializer.InitializeAsync();

        app.Run();
    }
}
