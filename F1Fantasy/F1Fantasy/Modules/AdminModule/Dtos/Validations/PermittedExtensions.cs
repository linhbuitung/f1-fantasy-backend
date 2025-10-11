using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AdminModule.Dtos.Validations;

public class PermittedExtensions : ValidationAttribute
{
    private readonly string[] _extensions;
    public PermittedExtensions(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (!_extensions.Contains(extension))
            {
                return new ValidationResult($"This file extension is not permitted.");
            }
        }
        return ValidationResult.Success;
    }
}