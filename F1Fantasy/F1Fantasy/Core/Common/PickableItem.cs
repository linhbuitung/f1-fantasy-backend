using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Fantasy.Core.Common;

[Table("pickable_item")]
public class PickableItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; } = 1;
    
    public virtual ICollection<Driver> Drivers { get; set; }
    
    public virtual ICollection<Constructor> Constructors { get; set; }
}