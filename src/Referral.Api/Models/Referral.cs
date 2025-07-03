using System.ComponentModel.DataAnnotations;

namespace Referral.Api.Models;

public class Referral
{
    public Guid? Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }
    public Guid? ProfileId { get; set; }
    public string? FullName { get; set; }
    public string? Status { get; set; } = "PENDING";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? UpdatedAt { get;set; }
}
