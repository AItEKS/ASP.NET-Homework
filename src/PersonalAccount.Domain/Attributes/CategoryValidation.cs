using System.ComponentModel.DataAnnotations;

public class CategoryNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var categoryName = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return new ValidationResult("Название категории обязательно");
        }

        // Проверка: допустимая длина
        if (categoryName.Length < 2 || categoryName.Length > 100)
        {
            return new ValidationResult("Длина названия категории должна быть от 2 до 100 символов");
        }

        return ValidationResult.Success;
    }
}