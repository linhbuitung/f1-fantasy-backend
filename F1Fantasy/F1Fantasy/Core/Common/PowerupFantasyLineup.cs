using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("powerup_fantasy_lineup")]
    public class PowerupFantasyLineup
    {
        [Key, Column(Order = 1)]
        public Guid FantasyLineupId { get; set; }

        [Key, Column(Order = 2)]
        public Guid PowerupId { get; set; }

        [ForeignKey(nameof(FantasyLineupId))]
        public FantasyLineup FantasyLineup { get; set; }

        [ForeignKey(nameof(PowerupId))]
        public Powerup Powerup { get; set; }
    }
}