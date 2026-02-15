using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class OrgAddressAttribute : ValidationAttribute
{
    private const string Pattern = @"^(\d{6}[\s,]+)?[а-яА-ЯёЁ0-9\s.,\-\/№()]+$";

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

public class OrgNameAttribute : ValidationAttribute
{
    private const string Pattern = 
        @"^(?:(?:ИП|ООО|АО|ПАО|НАО|ЗАО|ОАО|ТОВ|СПК|КФХ|ТСЖ|ЖСК|ПК|НКО|" +
        @"ФГУП|МУП|ГБУ|ГАУ|ФНПР|ФГБУ|ФГБУП|ФКУП|ФПП|ФППП|ФГКУП|ФГП|ФПП|ПП|ГП|" +
        @"ФПП|ФГБУП|ФГБУ|ФКУ|ФГКУ)\s+)?(?:""|«|'|\\()?[А-ЯЁ][а-яёА-ЯЁ\s\-\d\.\(\)\\\/]*?(?:""|»|'|\\))?\s*$";

    private static readonly Regex _orgNameRegex = new Regex(
        Pattern,
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var orgName = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(orgName))
        {
            return new ValidationResult("Название организации обязательно для заполнения");
        }

        // Проверка: допустимая длина
        if (orgName.Length > 255 || orgName.Length < 5)
        {
             return new ValidationResult("Длина названия должна быть от 5 до 255 символов");
        }

        // Проверка: поле соответсвует стандарту
        if (!_orgNameRegex.IsMatch(orgName!))
        {
            return new ValidationResult("Некорректный формат названия организации");
        }

        return ValidationResult.Success;
        
    }
}

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
        if (!inn.All(char.IsDigit))
        {
            return new ValidationResult("ИНН должен содержать только цифры");
        }

        // Проверка: допустимая длина
        if (inn.Length != 10 && inn!.Length != 12)
        {
            return new ValidationResult("Длина ИНН должна быть 10 или 12 символов");
        }

        return ValidationResult.Success;
    }
}