using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data;
using PersonalAccount.Data.Models;

namespace PersonalAccount.UnitTests;

[TestFixture]
public class DbContextTests
{   
    [Test]
    public async Task FetchOrganizations_PersonalAccountContext_Any()
    {
        var context = new PersonalAccountContext();

        var result = await context.Organizations.ToListAsync(CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Any());
    }
}