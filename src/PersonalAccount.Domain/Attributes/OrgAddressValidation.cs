using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class OrgAddressAttribute : ValidationAttribute
{
    private const string Pattern = @"^(\d{6})?[\s,]+[а-яА-ЯёЁ0-9\s.,\-\/№()]+$";

    private static readonly Regex _addressFormatRegex = new Regex(
        Pattern,
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var address = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(address))
        {
            return new ValidationResult("Адрес организации обязателен для заполнения");
        }

        // Проверка: допустимая длина
        if (address.Length < 10 || address.Length > 255)
        {
            return new ValidationResult("Длина адреса должна быть от 10 до 255 символов");
        }

        // Проверка: поле соответсвует стандарту
        if (!_addressFormatRegex.IsMatch(address))
        {
            return new ValidationResult("Адрес содержит недопустимые символы");
        }

        return ValidationResult.Success;
    }
}