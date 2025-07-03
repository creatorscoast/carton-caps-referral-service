using Referral.Api.Models;

namespace Referral.Api.Services;

public interface IAccountService
{
    Task<bool> CreateAsync(Account account);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByReferralCodeAsync(string referralCode);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<bool> UpdateAsync(Account account);
    Task<bool> DeleteByIdAsync(Guid accountId);
    Task<IEnumerable<Account>> SearchByReferralCodeAsync(string searchTerm);
}
