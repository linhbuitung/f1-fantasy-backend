using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("driver")]
    public class Driver
    {
        public Driver(string givenName, string familyName, DateOnly dateOfBirth, string nationality, string code, string? imgUrl)
        {
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;

            Code = code;
            ImgUrl = imgUrl;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string GivenName { get; set; }

        [Required, MaxLength(300)]
        public string FamilyName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateOnly DateOfBirth { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }
        
        [Required, Range(1,100)]
        public int Price { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        //constructor
        public Driver() { }

        public Driver(string givenName, string familyName, DateOnly dateOfBirth, string code, string? imgUrl)
        {
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            Code = code;
            ImgUrl = imgUrl;
        }

        // Navigation properties
        [Required]
        public string CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }
        
        public int? PickableItemId { get; set; }
        
        [ForeignKey(nameof(PickableItemId))]
        public virtual PickableItem? PickableItem { get; set; }

        public virtual ICollection<DriverPrediction> DriverPredictions { get; set; }
        
        public virtual ICollection<FantasyLineupDriver> FantasyLineupDrivers { get; set; }
        public virtual ICollection<RaceEntry> RaceEntries { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}