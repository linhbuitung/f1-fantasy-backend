using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.NotificationModule.Dtos.Create;

public class NotificationDto
{
    [Required]
    public int UserId { get; set; }
    [Required, MaxLength(200)]
    public required string Header { get; set; }

    [Required, MaxLength(1000)]
    public required string Content { get; set; }
    
    public DateTime? ReadAt { get; set; }
}