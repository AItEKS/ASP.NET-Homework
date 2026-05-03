using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Data.Models;
using PersonalAccount.Domain.Models.Dto;

using AppContext = PersonalAccount.Data.PersonalAccountContext;

namespace PersonalAccount.UnitTests;

[TestFixture]
public class EntityExtractorTests
{
    private AppContext _context = null!;
    private EntityExtractor _extractor = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppContext(options);
        _extractor = new EntityExtractor(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task ExtractNewEmployeesAsync_ShouldReturnOnlyMissingInDb()
    {
        var existingEmployee = new Emploee 
        { 
            Id = Guid.NewGuid(), 
            Code = 10, 
            Name = "Старый сотрудник", 
            Phone = "123", 
            CompanyId = Guid.NewGuid() 
        };
        _context.Emploees.Add(existingEmployee);
        await _context.SaveChangesAsync();

        var incomingRows = new List<JournalRowDto>
        {
            new JournalRowDto { EmploeeCode = 10, EmploeeName = "Старый сотрудник" }, 
            new JournalRowDto { EmploeeCode = 20, EmploeeName = "Новый сотрудник" }   
        };

        var newEmployees = (await _extractor.ExtractNewEmployeesAsync(incomingRows, CancellationToken.None)).ToList();

        Assert.That(newEmployees, Has.Count.EqualTo(1));
        
        var extractedEmployee = newEmployees.First();
        Assert.That(extractedEmployee.Code, Is.EqualTo(20));
        Assert.That(extractedEmployee.Name, Is.EqualTo("Новый сотрудник"));
    }
}