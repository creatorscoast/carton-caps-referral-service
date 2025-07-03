namespace Referral.Api.ViewModel;

public class ReferralCreateRequest
{
    public Guid AccountId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
