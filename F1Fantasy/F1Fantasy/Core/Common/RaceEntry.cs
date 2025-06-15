using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("race_entry")]
    public class RaceEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int? Position { get; set; }
        public int? Grid { get; set; }
        public int? FastestLap { get; set; }

        [Required]
        public int PointsGained { get; set; }

        [Required]
        public Guid DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; }

        [Required]
        public Guid RaceId { get; set; }

        [ForeignKey(nameof(RaceId))]
        public Race Race { get; set; }
    }
}