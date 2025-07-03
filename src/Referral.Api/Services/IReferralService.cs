using Referral.Api.ViewModel;

namespace Referral.Api.Services;

public interface IReferralService
{
    Task<bool> CreateAsync(Models.Referral referral);
    Task<Models.Referral?> GetByIdAsync(Guid id);
    Task<IEnumerable<Models.Referral>> GetAccountReferralsAsync(Guid accountId);
    Task<bool> UpdateAsync(Models.Referral referral);
}

