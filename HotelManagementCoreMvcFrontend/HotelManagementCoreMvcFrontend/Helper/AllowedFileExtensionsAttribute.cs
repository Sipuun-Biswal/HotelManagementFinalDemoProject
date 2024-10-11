using System.ComponentModel.DataAnnotations;
namespace HotelManagementCoreMvcFrontend.Helper
{
    public class AllowedFileExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public AllowedFileExtensionsAttribute(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult("Please upload a valid image file (jpg, jpeg, png, gif).");
                }
            }
            return ValidationResult.Success;
        }

    }
}
