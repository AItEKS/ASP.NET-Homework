using System.ComponentModel.DataAnnotations;

public class InnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var inn = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(inn))
        {
            return new ValidationResult("ИНН обязателен для заполнения");
        }
        
        // Проверка: поле состоит из цифр 
        if (inn!.All(char.IsDigit))
        {
            return new ValidationResult("ИНН должен содержать только цифры");
        }

        // Проверка: допустимая длина
        if (inn!.Length != 10 && inn!.Length != 12)
        {
            return new ValidationResult("Длина ИНН должна быть 10 или 12 символов");
        }

        return ValidationResult.Success;
    }
}