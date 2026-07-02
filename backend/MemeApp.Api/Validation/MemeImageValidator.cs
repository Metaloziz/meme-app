using MemeApp.Api.Constants;

namespace MemeApp.Api.Validation;

public static class MemeImageValidator
{
    public static ValidationResult ValidateTextFields(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            return ValidationResult.Invalid("Название и описание обязательны.");
        }

        return ValidationResult.Valid();
    }

    public static ValidationResult ValidateRequiredImage(IFormFile image)
    {
        if (image.Length == 0)
        {
            return ValidationResult.Invalid("Изображение обязательно.");
        }

        return ValidateImageContent(image);
    }

    public static ValidationResult ValidateOptionalImage(IFormFile? image)
    {
        if (image is null)
        {
            return ValidationResult.Valid();
        }

        if (image.Length == 0)
        {
            return ValidationResult.Invalid("Изображение не может быть пустым.");
        }

        return ValidateImageContent(image);
    }

    private static ValidationResult ValidateImageContent(IFormFile image)
    {
        if (image.Length > MemeConstants.MaxImageBytes)
        {
            return ValidationResult.Invalid("Максимальный размер изображения — 2 MB.");
        }

        if (!MemeConstants.AllowedContentTypes.Contains(image.ContentType))
        {
            return ValidationResult.Invalid("Допустимые форматы: JPEG, PNG, WebP.");
        }

        return ValidationResult.Valid();
    }
}
