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

        [ForeignKey("FantasyLineupId")]
        public FantasyLineup FantasyLineup { get; set; }

        [ForeignKey("PowerupId")]
        public Powerup Powerup { get; set; }
    }
}