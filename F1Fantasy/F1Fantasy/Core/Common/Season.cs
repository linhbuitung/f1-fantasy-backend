using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Fantasy.Core.Common;

[Table("season")]
public class Season
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    public bool IsActive { get; set; }
    
    // Navigation property
    public virtual ICollection<Race> Races { get; set; }
}