using System.ComponentModel.DataAnnotations;

namespace Referral.Api.Models;

public class Account
{
    [Required]
    public Guid Id { get; set; }
    public string? ReferralCode { get; set; }
    public int ReferralsSent => Referrals?.Where(w => w.Status.Equals("PENDING"))?.Count() ?? 0;
    public int ReferralsAccepted => Referrals?.Where(w => w.Status.Equals("COMPLETED"))?.Count() ?? 0;
    public int RewardPoints { get; set; }   
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();
}
