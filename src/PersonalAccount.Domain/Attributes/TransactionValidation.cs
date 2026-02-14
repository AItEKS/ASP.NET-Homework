using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.Domain.Validation;

public class NotZeroAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (!decimal.TryParse(value.ToString(), out var amount))
        {
            return new ValidationResult("Значение должно быть числом");
        }

        // Проверка: значение не равно 0
        if (amount == 0)
        {
            return new ValidationResult("Количество товара не может быть равно нулю");
        }

        return ValidationResult.Success;
    }
}

public class PastOrPresentAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var date = (DateTime)value;

        // Проверка: дата не больше текущей (плюс 1 минута на погрешность часов)
        if (date > DateTime.Now.AddMinutes(1))
        {
            return new ValidationResult("Дата операции не может быть в будущем");
        }

        return ValidationResult.Success;
    }
}

public class PositiveMoneyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (!decimal.TryParse(value.ToString(), out var money))
        {
            return new ValidationResult("Значение должно быть числом");
        }

        // Проверка: сумма не отрицательная
        if (money < 0)
        {
            return new ValidationResult("Сумма операции не может быть отрицательной");
        }

        return ValidationResult.Success;
    }
}