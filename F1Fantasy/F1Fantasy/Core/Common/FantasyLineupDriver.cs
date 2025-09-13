using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("fantasy_lineup_driver")]
    public class FantasyLineupDriver
    {
        [Key, Column(Order = 1)]
        public int FantasyLineupId { get; set; }

        [Key, Column(Order = 2)]
        public int DriverId { get; set; }
        
        public virtual FantasyLineup FantasyLineup { get; set; }
        public virtual Driver Driver { get; set; }
    }
}