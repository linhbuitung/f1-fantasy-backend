using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("race")]
    public class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("date", TypeName = "date")]
        public DateTime RaceDate { get; set; }

        [Required]
        [Column("deadline_date", TypeName = "date")]
        public DateTime DeadlineDate { get; set; }

        [Required]
        public bool Calculated { get; set; }

        [Required]
        public int CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public Circuit Circuit { get; set; }

        // Navigation properties
        public ICollection<RaceEntry> RaceEntries { get; set; }

        public ICollection<FantasyLineup> FantasyLineups { get; set; }
    }
}