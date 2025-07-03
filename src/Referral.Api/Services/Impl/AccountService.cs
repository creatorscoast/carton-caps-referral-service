using Dapper;
using Referral.Api.Core;
using Referral.Api.Data;
using Referral.Api.Models;

namespace Referral.Api.Services.Impl;

public sealed class AccountService : IAccountService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AccountService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> CreateAsync(Account account)
    {
        var existingAccount = await GetByIdAsync(account.Id);
        if (existingAccount is not null) return false;

        // Note: this is meant to represent microservices architecture in which accountId is coming
        // from a different service and is passed by eventing system or via api for eventual consistency.
        // The role of this service is everything related to referrals so the logic to generate the code
        // resides here. This code does not handle collisions or unique key errors. Those can be solve
        // by changing algorithm or increasing the length of the referral code.
        account.ReferralCode = ReferralCodeGenerator.EncodeGuidToBase26Unique(account.Id);

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Accounts (Id, ReferralCode, RewardPoints, IsDeleted)
VALUES (@Id, @ReferralCode, @RewardPoints, @IsDeleted)",
            account);

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid accountId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var result = await connection.ExecuteAsync(@"UPDATE Accounts
        SET IsDeleted = 1
        WHERE Id = @Id", new { Id = accountId });

        return result > 0;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = @"
        SELECT * FROM Accounts;
        SELECT * FROM Referrals";

        using var multi = await connection.QueryMultipleAsync(sql);
        var accounts = await multi.ReadAsync<Account>();

        if (accounts.Any())
        {
            var referrals = (await multi.ReadAsync<Models.Referral>())
                .GroupBy(g => g.AccountId)
                .ToDictionary(k => k.Key, v => v.ToList());
          
            foreach (var account in accounts) 
                if (referrals.ContainsKey(account.Id))
                    account.Referrals = referrals[account.Id];
        }

        return accounts;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<Account>("SELECT * FROM Accounts WHERE Id = @Id LIMIT 1",
            new { Id = id });
    }

    public async Task<Account?> GetByReferralCodeAsync(string referralCode)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<Account>("SELECT * FROM Accounts WHERE ReferralCode = @ReferralCode LIMIT 1",
            new { ReferralCode = referralCode });
    }

    public async Task<IEnumerable<Account>> SearchByReferralCodeAsync(string searchTerm)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QueryAsync<Account>(
            "SELECT * FROM Accounts WHERE ReferralCode LIKE '%' || @SearchTerm || '%'",
            new { SearchTerm = searchTerm });
    }

    public async Task<bool> UpdateAsync(Account account)
    {
        var existingAccount = await GetByIdAsync(account.Id);
        if (existingAccount is null) return false;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(@"UPDATE Accounts
            SET RewardPoints = @RewardPoints,
                IsDeleted = @IsDeleted
            WHERE Id = @Id;", account);

        return result > 0;
    }
}
