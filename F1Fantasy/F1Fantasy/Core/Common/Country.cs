using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Fantasy.Core.Common
{
    [Table("country")]
    public class Country
    {
        [Key]
        [MaxLength(100)]
        public string Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ShortName { get; set; }

        [Required]
        public List<string> Nationalities { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Constructor> Constructors { get; set; }

        public virtual ICollection<Driver> Drivers { get; set; }

        public ICollection<Circuit> Circuits { get; set; }
    }
}