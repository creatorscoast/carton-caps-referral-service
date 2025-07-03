
using FluentValidation;
using FluentValidation.Results;
using Referral.Api.Services;
using Referral.Api.Services.Impl;
using Referral.Api.ViewModel;

namespace Referral.Api.Endpoints.Impl;

public sealed class ReferralEndpoints : IEndpoint
{
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IReferralService, ReferralService>();
        services.AddSingleton<IRewardService, RewardService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var referrals = app.MapGroup("referrals")
            .WithTags("Referrals")
            .WithDescription("Create referrals and get referrals for an account.");

        referrals.MapPost("/", CreateReferralAsync).WithName("CreateReferral")
            .Accepts<ReferralCreateRequest>("application/json")
            .Produces<Models.Referral>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithSummary("Create a referral entry.");

        referrals.MapGet("/{accountId:guid}", GetReferralsAsync).WithName("GetReferrals")
            .Produces<IEnumerable<Models.Referral>>(200)
            .WithSummary("Get all referrals for an account with PENDING or COMPLETED status.");

        referrals.MapGet("/{id:guid}", GetReferralAsync).WithName("GetReferral")
            .Produces<Models.Referral>(200)
            .Produces(404)
            .WithSummary("Get a referral by Id.");

        referrals.MapPost("/{id:guid}/complete", CompleteReferralAsync).WithName("CompleteReferral")
            .Accepts<ReferralCompletedRequest>("application/json")
            .Produces(200)
            .Produces(404)
            .WithSummary("Completes a referral by referralId. Normally done via webhook.");
    }

    internal static async Task<IResult> CreateReferralAsync(ReferralCreateRequest request, 
        IAccountService accountService, 
        IReferralService referralService,
        IValidator<Models.Referral> validator) {
        var referral = new Models.Referral
        {
            AccountId = request.AccountId,
            FullName = $"{request.FirstName} {request.LastName}" 
        };
        var validationResult = await validator.ValidateAsync(referral);
        if (validationResult.IsValid == false)
            return Results.BadRequest(validationResult.Errors);

        var account = await accountService.GetByIdAsync(referral.AccountId);
        if (account is null) return Results.NotFound();    

        var created = await referralService.CreateAsync(referral);
        if (created) return Results.CreatedAtRoute("GetReferral", new { id = referral.Id }, referral);

        return Results.BadRequest(new List<ValidationFailure>{
                new ("Referral", "Unable to create referral")
        });
    }

    internal static async Task<IResult> GetReferralsAsync(Guid accountId, IReferralService referralService)
    {
        var referrals = await referralService.GetAccountReferralsAsync(accountId);

        return Results.Ok(referrals);
    }

    internal static async Task<IResult> GetReferralAsync(Guid id, IReferralService referralService)
    {
        var referral = await referralService.GetByIdAsync(id);

        return referral is not null ? Results.Ok(referral) : Results.NotFound();
    }

    internal static async Task<IResult> CompleteReferralAsync(Guid id, 
        ReferralCompletedRequest request, 
        IReferralService referralService,
        IAccountService accountService,
        IRewardService rewardService)
    {
        var referral = await referralService.GetByIdAsync(id);
        if (referral is null) return Results.NotFound();

        referral.ProfileId = request.ProfileId;
        referral.FullName = $"{request.FirstName} {request.LastName}";
        referral.Status = "COMPLETED";
        referral.UpdatedAt = DateTimeOffset.Now;
       
        var updated = await referralService.UpdateAsync(referral);

        if (updated)
        {
            var account = await accountService.GetByIdAsync(referral.AccountId);
            account.RewardPoints =
                await rewardService.GetRewardPointsAsync(account.ReferralCode, account.ReferralsAccepted);

            await accountService.UpdateAsync(account);
        }

        return updated ? Results.Ok(updated) : Results.NoContent();
    }
}
