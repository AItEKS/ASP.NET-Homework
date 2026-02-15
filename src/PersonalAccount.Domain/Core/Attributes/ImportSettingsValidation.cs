using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PersonalAccount.Domain.Validation;

public class BatchSizeAttribute : ValidationAttribute
{
    private const int MinSize = 1;
    private const int MaxSize = 10000;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
             return ValidationResult.Success;
        }

        if (!int.TryParse(value.ToString(), out var size))
        {
            return new ValidationResult("Размер пакета должен быть целым числом");
        }

        // Проверка: значение не меньше минимума
        if (size < MinSize)
        {
             return new ValidationResult($"Размер пакета не может быть меньше {MinSize}");
        }

        // Проверка: значение не больше максимума
        if (size > MaxSize)
        {
             return new ValidationResult($"Размер пакета не может быть больше {MaxSize}");
        }

        return ValidationResult.Success;
    }
}

public class ValidJsonAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Проверка: поле не пустое
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Конфигурация маппинга (JSON) обязательна");
        }

        var jsonString = value.ToString()!;

        try
        {
            // Проверка: попытка прочитать структуру JSON
            JsonDocument.Parse(jsonString);
        }
        catch (JsonException)
        {
            return new ValidationResult("Поле должно содержать валидный JSON формат");
        }

        return ValidationResult.Success;
    }
}