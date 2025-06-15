using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace F1Fantasy.Core.Common
{
    [Table("prediction")]
    public class Prediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("datePredicted", TypeName = "date")]
        public DateTime DatePredicted { get; set; }

        [Required]
        public int PredictYear { get; set; }

        [Required]
        public bool Rain { get; set; }

        //[Required, MaxLength(10)]
        //public string PredictionType { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile User { get; set; }

        [Required]
        public int CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public Circuit Circuit { get; set; }

        // Navigation property
        public ICollection<DriverPrediction> DriverPredictions { get; set; }
    }
}