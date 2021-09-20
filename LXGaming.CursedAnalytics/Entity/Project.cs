using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LXGaming.CursedAnalytics.Entity {

    public class Project {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; init; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Slug { get; set; }
    }
}