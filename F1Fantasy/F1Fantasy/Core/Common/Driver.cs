using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("driver")]
    public class Driver
    {
        public Driver(string givenName, string familyName, DateTime dateOfBirth, string nationality, string code, string? imgUrl)
        {
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(300)]
        public string GivenName { get; set; }

        [Required, MaxLength(300)]
        public string FamilyName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(200)]
        public string Nationality { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        // Navigation properties

        public ICollection<DriverPrediction> DriverPredictions { get; set; }
        public ICollection<FantasyLineupDriver> FantasyLineupDrivers { get; set; }
        public ICollection<RaceEntry> RaceEntries { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}