using System.ComponentModel.DataAnnotations;

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