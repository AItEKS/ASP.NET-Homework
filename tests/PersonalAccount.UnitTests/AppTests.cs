using System.Reflection;
using System.Text.RegularExpressions;
using PersonalAccount.Domain;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.UnitTests;

public class Tests
{
    [Test]
    public void Create_Category_CheckNullName()
    {
        // Подготовка
        var domain = new Category()
        {
            Name = "Boom"
        };

        // Действие

        // Проверка
        Assert.That (domain.Name is not null);
    }

    [Test]
    public void Create_Category_ExistsAttributes()
    {
        // Подготовка
        var domain = new Category()
        {
            Name = "Boom"
        };
        var type = typeof(Category);

        // Действие
        var properties = type.GetProperties().Where(x => x.GetCustomAttributes(true).Any());

        // Проверка
        Assert.That (properties.Any());
    }

    [Test]
    public void Create_Employee_ExistsPhomeTemplateAttr()
    {
        // Подготовка
        var domain = new Employee()
        {
            Name = "Tom",
            Phone = "89041307795"
        };

        // Действие
        var properties = domain.GetType().GetProperties().Where(x => x.GetCustomAttribute<PhoneTemplateAttribute>(true) is not null );

        // Проверка
        Assert.That (properties.Any());
        var attribute = properties.First().GetCustomAttribute<PhoneTemplateAttribute>();
        Assert.That(!string.IsNullOrEmpty(attribute!.Template));

        var match = new Regex(attribute!.Template);
        Assert.That(match.IsMatch(domain.Phone!) == true);
    }
}
