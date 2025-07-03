
namespace Referral.Api.Services.Impl;

public sealed class RewardService : IRewardService
{
    /// <summary>
    /// This is a fake rewards calculator. In practice it could be another service
    /// implementing some rule engine or internal to referall but with
    /// business logic 
    /// </summary>
    /// <param name="referralCode"></param>
    /// <param name="completedCount"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<int> GetRewardPointsAsync(string referralCode, int completedCount)
    {
        var result = completedCount * GetMultiplier(completedCount);
        
        return Task.FromResult((int)result);
    }

    private float GetMultiplier(int completedCount) => completedCount switch
    {
        < 100 => 1f,
        < 1000 and > 100 => 2f,
        > 1000 => 3f
    };
}
