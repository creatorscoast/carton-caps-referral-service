using FluentValidation;

namespace Referral.Api.Validators;

public sealed class ReferralValidator : AbstractValidator<Models.Referral>
{
    public ReferralValidator()
    {
        RuleFor(referral => referral.AccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
