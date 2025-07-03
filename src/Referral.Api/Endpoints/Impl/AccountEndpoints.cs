
using FluentValidation;
using FluentValidation.Results;
using Referral.Api.Models;
using Referral.Api.Services;
using Referral.Api.Services.Impl;
using Referral.Api.ViewModel;

namespace Referral.Api.Endpoints.Impl;

/// <summary>
/// Account endpoints are used internally to make sure this 'service' has a mapping to
/// an account managed by idp or other service.
/// </summary>
public sealed class AccountEndpoints : IEndpoint
{
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAccountService, AccountService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var accounts = app.MapGroup("accounts")
            .WithTags("Account")
            .WithDescription(@"Account/User are managed by a seperate IDP or service. 
                This endpoints creates an account in the referral service and generates their referral code.");

        accounts.MapPost("/", CreateAccountAsync).WithName("CreateAccount")
            .Accepts<AccountCreateRequest>("application/json")
            .Produces<Account>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithSummary("Create a referral code for account/user.");

        accounts.MapGet("/", GetAccountsAsync).WithName("GetAccounts")
            .Produces<IEnumerable<Account>>(200)
            .WithSummary("Get all accounts and their referral code.");

        accounts.MapGet("/{id:guid}", GetAccountAsync).WithName("GetAccount")
            .Produces<Account>(200)
            .Produces(404)
            .WithSummary("Get account and its referral code.");

        accounts.MapPut("/{id:guid}", UpdateAccountAsync).WithName("UpdateAccount")
            .Accepts<Account>("application/json")
            .Produces<Account>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .Produces(404)
            .WithSummary("Update account statistics.");

        accounts.MapDelete("/{id}", DeleteAccountAsync).WithName("DeleteAccount")
            .Produces(204)
            .Produces(404)
            .WithSummary("Soft delete of a referral account.");
    }

    internal static async Task<IResult> CreateAccountAsync(AccountCreateRequest request, IAccountService accountService,
               IValidator<Account> validator) {
        var account = new Account
        {
            Id = request.AccountId
        };
        var validationResult = await validator.ValidateAsync(account);
        if (validationResult.IsValid == false)
            return Results.BadRequest(validationResult.Errors);

        var created = await accountService.CreateAsync(account);
        if (created) return Results.CreatedAtRoute("GetAccount", new { id = account.Id }, account);

        return Results.BadRequest(new List<ValidationFailure>{
                new ("AccountId", "Account already exist")
        });
    }

    internal static async Task<IResult> GetAccountsAsync(IAccountService accountService, string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
            return Results.Ok(await accountService.SearchByReferralCodeAsync(searchTerm));

        var accounts = await accountService.GetAllAsync();

        return Results.Ok(accounts);
    }

    internal static async Task<IResult> GetAccountAsync(Guid id, IAccountService accountService,
        IReferralService referralService)
    {
        var account = await accountService.GetByIdAsync(id);
        var referrals = await referralService.GetAccountReferralsAsync(id);

        if (account is not null)
            account.Referrals = referrals.ToList();

        return account is not null ? Results.Ok(account) : Results.NotFound();
    }

    internal static async Task<IResult> UpdateAccountAsync(Guid id, Account account, IAccountService accountService,
          IValidator<Account> validator)
    {
        account.Id = id;

        var validationResult = await validator.ValidateAsync(account);
        if (validationResult.IsValid == false)
            return Results.BadRequest(validationResult.Errors);

        var updated = await accountService.UpdateAsync(account);
        if (updated)
            return Results.Ok(updated);

        return Results.NotFound();
    }

    internal static async Task<IResult> DeleteAccountAsync(Guid id, IAccountService accountService)
    {
        var deleted = await accountService.DeleteByIdAsync(id);

        return deleted == true ? Results.NoContent() : Results.NotFound();
    }
}
