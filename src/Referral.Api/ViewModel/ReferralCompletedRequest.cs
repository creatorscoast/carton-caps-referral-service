using System.ComponentModel.DataAnnotations;

namespace Referral.Api.ViewModel;

public class ReferralCompletedRequest
{
    public Guid? ProfileId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }
}
