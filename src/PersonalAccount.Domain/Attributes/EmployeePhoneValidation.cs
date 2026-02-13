using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class EmployeePhoneAttribute : ValidationAttribute
{
    private const string Pattern = @"^(?:(?:\+?7|8|7)?[\s\-\.]?)?(?:\(?[0-9]{3,5}\)?[\s\-\.]?)?[0-9]{3}[\s\-\.]*[0-9]{2}[\s\-\.]*[0-9]{2}$";
    
    private static readonly Regex _employeePhoneRegex = new Regex(
        Pattern,
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var employeePhone = value as string;

        // Проверка: допустимая длина
        if (employeePhone?.Count(char.IsDigit) != 11)
        {
             return new ValidationResult("Телефон должен содержать 11 цифр");
        }

        // Проверка: поле соответсвует стандарту
        if (!_employeePhoneRegex.IsMatch(employeePhone!))
        {
            return new ValidationResult("Некорректный формат телефона");
        }

        return ValidationResult.Success;
        
    }
}