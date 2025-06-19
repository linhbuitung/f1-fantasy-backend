using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    public class Nationality
    {
        [Key]
        [MaxLength(100)]
        public string NationalityId { get; set; }

        [Required]
        public List<string> Names { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<Constructor> Constructors { get; set; }

        public ICollection<Driver> Drivers { get; set; }
    }
}