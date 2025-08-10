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
        public int TransferPointsDeducted { get; set; }

        [Required]
        public int PointsGained { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public int RaceId { get; set; }

        [ForeignKey(nameof(RaceId))]
        public Race Race { get; set; }

        // Navigation properties
        public ICollection<FantasyLineupDriver> FantasyLineupDrivers { get; set; }

        public ICollection<PowerupFantasyLineup> PowerupFantasyLineups { get; set; }
    }
}