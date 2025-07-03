using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Referral.Api.Models;

namespace Referral.Api.Tests.Integration;

public class AccountEndpointsTests : IClassFixture<WebApplicationFactory<Constants>>,
    IAsyncLifetime
{
    private readonly WebApplicationFactory<Constants> _factory;

    public AccountEndpointsTests(WebApplicationFactory<Constants> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAccount_CreatesAccount_WithRequiredData()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var account = new Account
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await httpClient.PostAsJsonAsync("/accounts", account);
        var createdAccount = await result.Content.ReadFromJsonAsync<Account>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        createdAccount.Id.Should().Be(account.Id);
        createdAccount.ReferralCode.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"http://localhost/accounts/{account.Id}");
    }

    [Fact]
    public async Task CreateAccount_Fails_WithInvalidId()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var account = new Account
        {
            Id = Guid.Empty
        };

        // Act
        var result = await httpClient.PostAsJsonAsync("/accounts", account);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors.First()!;

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Id");
        error.ErrorMessage.Should().Be("'Id' must not be empty.");
    }

    [Fact]
    public async Task GetAccount_ReturnsAccount_WhenExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var account = new Account
        {
            Id = Guid.NewGuid()
        };
        await httpClient.PostAsJsonAsync("/accounts", account);

        // Act
        var result = await httpClient.GetAsync($"/accounts/{account.Id}");
        var existingAccount = await result.Content.ReadFromJsonAsync<Account>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        existingAccount.Id.Should().Be(account.Id);
        existingAccount.ReferralCode.Should().NotBeEmpty();
    }

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();

        // Note: here we can call delete since we are using sql lite with soft delete
        // for this exercise it doesn't matter too much.
    }

    public Task InitializeAsync() => Task.CompletedTask;
}
