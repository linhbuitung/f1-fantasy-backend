using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Fantasy.Core.Common;

[Table("fantasy_lineup_constructor")]
public class FantasyLineupConstructor
{
    [Key, Column(Order = 1)]
    public int FantasyLineupId { get; set; }

    [Key, Column(Order = 2)]
    public int ConstructorId { get; set; }
        
    [ForeignKey(nameof(FantasyLineupId))]
    public virtual FantasyLineup FantasyLineup { get; set; }
        
    [ForeignKey(nameof(ConstructorId))]
    public virtual Constructor Constructor { get; set; }
}