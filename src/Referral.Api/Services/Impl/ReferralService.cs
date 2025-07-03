using Dapper;
using Referral.Api.Data;

namespace Referral.Api.Services.Impl;

public sealed class ReferralService : IReferralService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReferralService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> UpdateAsync(Models.Referral referral)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"UPDATE Referrals
          SET ProfileId = @ProfileId,
              FullName = @FullName,
              Status = @Status,
              CreatedAt = @CreatedAt,
              UpdatedAt = @UpdatedAt
          WHERE Id = @Id;",
            referral);

        return result > 0;
    }

    public async Task<bool> CreateAsync(Models.Referral referral)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Referrals (Id, AccountId, ProfileId, FullName, Status, CreatedAt, UpdatedAt)
VALUES (@Id, @AccountId, @ProfileId, @FullName, @Status, @CreatedAt, @UpdatedAt);",
            referral);

        return result > 0;
    }

    public async Task<IEnumerable<Models.Referral>> GetAccountReferralsAsync(Guid accountId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QueryAsync<Models.Referral>("SELECT * FROM Referrals WHERE AccountId = @Id",
            new { Id = accountId });
    }

    public async Task<Models.Referral?> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QueryFirstOrDefaultAsync<Models.Referral>("SELECT * FROM Referrals WHERE Id = @Id",
            new { Id = id });
    }
}
