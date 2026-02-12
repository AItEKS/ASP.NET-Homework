using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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