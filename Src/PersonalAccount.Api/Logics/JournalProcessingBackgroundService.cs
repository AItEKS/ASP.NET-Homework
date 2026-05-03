using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonalAccount.Common.Core;
using PersonalAccount.Data;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Фоновый сервис API. Постоянно проверяет плоскую таблицу JournalRows
/// </summary>
public class JournalProcessingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public JournalProcessingBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[ETL Сервис] Запущен фоновый процесс обработки журнала.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PersonalAccountContext>();
                var extractor = scope.ServiceProvider.GetRequiredService<IEntityExtractor>();
                var transformer = scope.ServiceProvider.GetRequiredService<IDataTransformerService>();
                var businessRepo = scope.ServiceProvider.GetRequiredService<IBusinessDataRepository>();

                var rawRows = await context.JournalRows
                    .AsNoTracking()
                    .Take(1000)
                    .ToListAsync(stoppingToken);

                if (rawRows.Any())
                {
                    Console.WriteLine($"[ETL Сервис] Найдено {rawRows.Count} необработанных строк. Начинаем трансформацию...");

                    var dtos = rawRows.Select(x => new JournalRowDto
                    {
                        Code = x.Transnumber ?? 0,
                        TypeCode = x.Transtype ?? 0,
                        ReceiptNumber = x.Receiptn ?? 0,
                        ProductCode = x.Productid,
                        CategoryCode = x.Categoryid,
                        EmploeeCode = x.Emploeeid,
                        Period = x.Dater ?? DateTime.UtcNow,
                        Quantity = x.Quantity ?? 0,
                        Price = x.Price ?? 0,
                        Discount = x.Discountamount ?? 0,
                        EmploeeName = x.EmploeeName,
                        CategoryName = x.CategoryName,
                        ProductName = x.ProductName
                    }).ToList();

                    var newCategories = (await extractor.ExtractNewCategoriesAsync(dtos, stoppingToken)).ToList();
                    await businessRepo.SaveCategoriesAsync(newCategories, stoppingToken);

                    var newNomenclature = (await extractor.ExtractNewNomenclatureAsync(dtos, stoppingToken)).ToList();
                    await businessRepo.SaveNomenclatureAsync(newNomenclature, stoppingToken);

                    var newEmployees = (await extractor.ExtractNewEmployeesAsync(dtos, stoppingToken)).ToList();
                    await businessRepo.SaveEmployeesAsync(newEmployees, stoppingToken);

                    var allNom = await context.Nomenclatures.Select(n => new NomenclatureModel { Id = n.Id, Code = n.Code, Name = n.Name }).ToListAsync(stoppingToken);
                    var allEmp = await context.Emploees.Select(e => new EmploeeModel { Id = e.Id, Code = e.Code, Name = e.Name }).ToListAsync(stoppingToken);

                    var branch = new BranchModel { Id = Guid.NewGuid(), Name = "Системный" };

                    var transactions = transformer.MapToDomain(branch, dtos, allEmp, allNom).ToList();

                    await businessRepo.SaveTransactionsAsync(transactions, stoppingToken);

                    var codesToDelete = rawRows.Select(r => r.Transnumber).ToList();
                    await context.JournalRows
                        .Where(r => codesToDelete.Contains(r.Transnumber))
                        .ExecuteDeleteAsync(stoppingToken);

                    Console.WriteLine($"[ETL Сервис] Успешно перенесено {transactions.Count} транзакций в боевые таблицы.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ETL ОШИБКА] {ex.Message}");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}