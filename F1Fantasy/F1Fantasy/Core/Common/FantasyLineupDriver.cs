using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("fantasy_lineup_driver")]
    public class FantasyLineupDriver
    {
        [Key, Column(Order = 1)]
        public Guid FantasyLineupId { get; set; }

        [Key, Column(Order = 2)]
        public Guid DriverId { get; set; }

        [ForeignKey(nameof(FantasyLineupId))]
        public FantasyLineup FantasyLineup { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; }
    }
}