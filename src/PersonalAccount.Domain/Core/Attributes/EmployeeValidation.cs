using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class EmployeeNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var employeeName = value as string;

        // Проверка: поле не пустое
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            return new ValidationResult("Имя сотрудника обязательно");
        }

        // Проверка: допустимая длина
        if (employeeName.Length < 5 || employeeName.Length > 50)
        {
            return new ValidationResult("Длина имени должна быть от 5 до 50 символов");
        }

        return ValidationResult.Success;
    }
}

public class EmployeePhoneAttribute : ValidationAttribute
{
    private const string Pattern = @"^(?:(?:\+?7|8|7)?[\s\-\.]?)?(?:\(?[0-9]{3,5}\)?[\s\-\.]?)?[0-9]{3}[\s\-\.]*[0-9]{2}[\s\-\.]*[0-9]{2}$";
    
    private static readonly Regex _employeePhoneRegex = new Regex(
        Pattern,
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var employeePhone = value as string;

        if (string.IsNullOrEmpty(employeePhone))
        {
            return ValidationResult.Success;
        }

        // Проверка: допустимая длина
        if (employeePhone.Where(char.IsDigit).Count() != 11)
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