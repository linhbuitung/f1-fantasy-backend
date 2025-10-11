using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Validations;

public class MaxFileSize : ValidationAttribute
{
    private readonly int _maxFileSizeInBytes;
    public MaxFileSize(int maxFileSizeInBytes)
    {
        _maxFileSizeInBytes = maxFileSizeInBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null && file.Length > _maxFileSizeInBytes)
        {
            return new ValidationResult($"Maximum allowed file size is {_maxFileSizeInBytes / 1024} KB.");
        }
        return ValidationResult.Success;
    }
}