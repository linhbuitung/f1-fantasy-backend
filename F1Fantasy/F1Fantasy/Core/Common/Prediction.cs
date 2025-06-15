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
        public Guid Id { get; set; }

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
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public Guid CircuitId { get; set; }

        [ForeignKey(nameof(CircuitId))]
        public Circuit Circuit { get; set; }

        // Navigation property
        public ICollection<DriverPrediction> DriverPredictions { get; set; }
    }
}