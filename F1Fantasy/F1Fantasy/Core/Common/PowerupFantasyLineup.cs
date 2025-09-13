using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("powerup_fantasy_lineup")]
    public class PowerupFantasyLineup
    {
        [Key, Column(Order = 1)]
        public int FantasyLineupId { get; set; }

        [Key, Column(Order = 2)]
        public int PowerupId { get; set; }

        public int? DriverId { get; set; }

        public virtual FantasyLineup FantasyLineup { get; set; }

        public virtual Powerup Powerup { get; set; }
        
        [ForeignKey(nameof(DriverId))]

        public virtual Driver? Driver { get; set; }
    }
}