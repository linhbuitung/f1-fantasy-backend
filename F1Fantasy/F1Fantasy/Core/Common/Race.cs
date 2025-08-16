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
        [Column(TypeName = "date")]
        public DateOnly RaceDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateOnly DeadlineDate { get; set; }

        [Required]
        public bool Calculated { get; set; }

        [Required]
        public int CircuitId { get; set; }

        [ForeignKey(nameof(CircuitId))]
        public Circuit Circuit { get; set; }
        
        [Required]
        public int SeasonId { get; set; }

        [ForeignKey(nameof(SeasonId))]
        public virtual Season Season { get; set; }

        // Navigation properties
        public virtual ICollection<RaceEntry> RaceEntries { get; set; }

        public virtual ICollection<FantasyLineup> FantasyLineups { get; set; }
    }
}