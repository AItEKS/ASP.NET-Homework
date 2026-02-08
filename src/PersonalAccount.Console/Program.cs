using PersonalAccount.Domain;

var helloUser = new HelloUser();
Console.WriteLine(helloUser.GetHelloWorld());

while (true)
{
    await Task.Delay(TimeSpan.FromHours(1));
}