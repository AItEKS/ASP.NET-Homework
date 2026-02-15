using System.ComponentModel.DataAnnotations;

public class ProductNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var productName = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(productName))
        {
            return new ValidationResult("Название товара обязательно");
        }

        // Проверка: допустимая длина
        if (productName.Length < 5 || productName.Length > 50)
        {
            return new ValidationResult("Длина товара должна быть от 5 до 50 символов");
        }

        return ValidationResult.Success;
    }
}

public class ProductPriceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Цена товара обязательна");
        }

        if (value is not decimal productPrice)
        {
            return new ValidationResult("Некорректный формат цены");
        }

        // Проверка: цена не отрицательная
        if (productPrice < 0)
        {
            return new ValidationResult("Цена не может быть отрицательной");
        }

        // Проверка: цена не равна нулю
        if (productPrice == 0)
        {
            return new ValidationResult("Цена должна быть больше нуля");
        }

        // Проверка: максимальная цена
        if (productPrice > 1_000_000)
        {
            return new ValidationResult("Цена не может превышать 1 000 000");
        }

        // Проверка: не более 2 знаков после запятой
        if (decimal.Round(productPrice, 2) != productPrice)
        {
            return new ValidationResult("Цена может содержать не более 2 знаков после запятой");
        }

        return ValidationResult.Success;
    }
}

public class UnitOfMeasureAttribute : ValidationAttribute
{
    private static readonly string[] AllowedUnits =
    {
        "шт", "кг", "г", "л", "мл", "м", "см", "мм",
        "м²", "м³", "уп", "компл", "пар", "т"
    };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Проверка: поле не пустое
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Единица измерения обязательна");
        }

        var unit = value.ToString()!.Trim().ToLower();

        // Проверка: допустимая длина
        if (unit.Length < 1 || unit.Length > 20)
        {
            return new ValidationResult("Длина единицы измерения должна быть от 1 до 20 символов");
        }

        // Проверка: входит в список допустимых значений
        if (!AllowedUnits.Contains(unit))
        {
            return new ValidationResult($"Недопустимая единица измерения. Допустимые значения: {string.Join(", ", AllowedUnits)}");
        }

        return ValidationResult.Success;
    }
}