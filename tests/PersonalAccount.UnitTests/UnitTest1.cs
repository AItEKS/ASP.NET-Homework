using PersonalAccount.Domain;

namespace PersonalAccount.UnitTests;

public class Tests
{
    [Test]
    public void Get_HelloWorld()
    {
        // Подготовка
        var helloUser = new HelloUser();

        // Действие
        var result = helloUser.GetHelloWorld();

        // Проверка
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }
}
