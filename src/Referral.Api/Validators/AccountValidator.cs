using FluentValidation;
using Referral.Api.Models;

namespace Referral.Api.Validators;

public sealed class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(account => account.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
