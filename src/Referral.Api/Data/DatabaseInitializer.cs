using Dapper;

namespace Referral.Api.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Accounts (
            Id TEXT PRIMARY KEY NOT NULL,
            ReferralCode TEXT NOT NULL,
            RewardPoints INTEGER NOT NULL,
            IsDeleted INTEGER NOT NULL DEFAULT 0 
        );");
        await connection.ExecuteAsync(@"CREATE UNIQUE INDEX IF NOT EXISTS IX_Accounts_ReferralCode ON Accounts (ReferralCode);");

        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Referrals (
                Id TEXT PRIMARY KEY NOT NULL,
                AccountId TEXT NOT NULL,
                ProfileId TEXT,
                FullName TEXT,
                Status TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT,
                FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
            );");
    }
}
