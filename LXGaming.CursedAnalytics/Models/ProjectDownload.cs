using System.ComponentModel.DataAnnotations;

namespace LXGaming.CursedAnalytics.Models;

public class ProjectDownload {

    [Key]
    public long Id { get; init; }

    [Required]
    public long ProjectId { get; init; }

    [Required]
    public DateTime Timestamp { get; init; }

    [Required]
    public long Value { get; init; }

    public virtual Project Project { get; init; } = null!;
}