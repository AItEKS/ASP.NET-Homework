using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain;
using PersonalAccount.Domain.Dto;
using PersonalAccount.Domain.Models;

var helloUser = new HelloUser();
Console.WriteLine(helloUser.GetHelloWorld());

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();
var options = configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException("Unabled loading appsettings.json");

var journalProvider = new JournalProvider(options.ConnectionString);
DateTime start = new DateTime(2020, 12, 9);
DateTime end = new DateTime(2020, 12, 10);

List<JournalEntryDto> result = journalProvider.GetTransactions(start, end);
Console.WriteLine(result.Count());
foreach (var record in result)
{
    Console.WriteLine($"{record.Id} - {record.Amount} - {record.Quantity} - {record.EmployeeCode}");
}

while (true)
{
    await Task.Delay(TimeSpan.FromHours(1));
}