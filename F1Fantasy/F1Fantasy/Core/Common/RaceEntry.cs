using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("race_entry")]
    public class RaceEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Position { get; set; }
        
        public int? Grid { get; set; }
        
        public int? FastestLap { get; set; }

        [Required]
        public int PointsGained { get; set; }
        
        public bool Finished { get; set; }

        [Required]
        public int DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public virtual Driver Driver { get; set; }

        [Required]
        public int RaceId { get; set; }

        [ForeignKey(nameof(RaceId))]
        public virtual Race Race { get; set; }
        
        [Required]
        public int ConstructorId { get; set; }

        [ForeignKey(nameof(ConstructorId))]
        public virtual Constructor Constructor { get; set; }
    }
}