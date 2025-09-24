using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace F1Fantasy.Core.Common
{
    [Table("fantasy_lineup")]
    public class FantasyLineup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TotalAmount { get; set; }

        [Required]
        public int TransfersMade { get; set; }

        [Required]
        public int PointsGained { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int RaceId { get; set; }

        [ForeignKey(nameof(RaceId))]
        public virtual Race Race { get; set; }

        // Navigation properties
        public virtual ICollection<FantasyLineupDriver> FantasyLineupDrivers { get; set; }
        
        public virtual ICollection<FantasyLineupConstructor> FantasyLineupConstructors { get; set; }
        
        public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineups { get; set; }
    }
}