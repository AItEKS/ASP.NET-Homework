using System.ComponentModel.DataAnnotations;

public class CheckNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var checkNumber = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(checkNumber))
        {
            return new ValidationResult("Номер чека обязателен");
        }

        // Проверка: допустимая длина (строго до 20 символов)
        if (checkNumber.Length > 20)
        {
            return new ValidationResult("Номер чека не может превышать 20 символов");
        }

        return ValidationResult.Success;
    }
}

public class DescriptionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var description = value as string;

        if (description == null)
        {
             return ValidationResult.Success;
        }

        // Проверка: допустимая длина (до 255 символов)
        if (description.Length > 255)
        {
            return new ValidationResult("Описание не может превышать 255 символов");
        }

        return ValidationResult.Success;
    }
}