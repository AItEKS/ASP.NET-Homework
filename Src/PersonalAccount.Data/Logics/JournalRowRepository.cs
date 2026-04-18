using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Data.Logics;

/// <summary>
/// Репозиторий для скоростной записи сырых данных в БД.
/// </summary>
public class JournalRowRepository : IJournalRowRepository
{
    private readonly PersonalAccountContext _context;

    public JournalRowRepository(PersonalAccountContext context)
    {
        _context = context;
    }

    public async Task BulkInsertAsync(IEnumerable<JournalRowDto> dtos, CancellationToken token)
    {
        if (dtos == null || !dtos.Any())
        {
            return;
        }

        var entities = dtos.Select(dto => new JournalRow
        {
            Code = dto.Code,
            TypeCode = dto.TypeCode,
            ReceiptNumber = dto.ReceiptNumber,
            ProductCode = dto.ProductCode,
            CategoryCode = dto.CategoryCode,
            EmploeeCode = dto.EmploeeCode,
            
            EmploeeName = dto.EmploeeName,
            CategoryName = dto.CategoryName,
            NomenclatureName = dto.NomenclatureName,
            
            Period = dto.Period,
            Quantity = dto.Quantity,
            Price = dto.Price,
            Discount = dto.Discount,
            
            UploadedAt = DateTime.UtcNow
        }).ToList();

        await _context.BulkInsertAsync(entities, cancellationToken: token);
    }
}