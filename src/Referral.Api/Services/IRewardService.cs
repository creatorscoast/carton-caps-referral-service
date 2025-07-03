namespace Referral.Api.Services;

public interface IRewardService
{
    Task<int> GetRewardPointsAsync(string referralCode,
        int completedCount);
}
