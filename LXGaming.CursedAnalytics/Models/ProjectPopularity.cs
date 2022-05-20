using System.ComponentModel.DataAnnotations;

namespace LXGaming.CursedAnalytics.Models; 

public class ProjectPopularity {
    
    [Key]
    public long Id { get; init; }

    [Required]
    public long ProjectId { get; init; }

    [Required]
    public DateTime Timestamp { get; init; }

    [Required]
    public long Rank { get; init; }

    [Obsolete]
    [Required]
    public decimal Score { get; init; }

    public virtual Project Project { get; init; } = null!;
}