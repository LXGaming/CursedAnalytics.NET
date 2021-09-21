using System;
using System.ComponentModel.DataAnnotations;

namespace LXGaming.CursedAnalytics.Entity {

    public class ProjectPopularity {

        [Key]
        public long Id { get; init; }

        [Required]
        public long ProjectId { get; init; }

        [Required]
        public DateTime Timestamp { get; init; }

        [Required]
        public long Rank { get; init; }

        [Required]
        public decimal Score { get; init; }

        public virtual Project Project { get; init; }
    }
}