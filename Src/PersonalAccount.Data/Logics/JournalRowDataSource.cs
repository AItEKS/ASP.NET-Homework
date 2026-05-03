using Microsoft.EntityFrameworkCore;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Models;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PersonalAccount.Data.Logics;

public class JournalRowDataSource : IJournalDataSource
{
    private readonly PersonalAccountContext _context;

    public JournalRowDataSource(PersonalAccountContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JournalRowDto>> GetUnprocessedRowsAsync(BranchModel branch, int batchSize, CancellationToken token)
    {
        return await _context.JournalRows
            .AsNoTracking()
            .Where(x => x.BranchId == branch.Id) 
            .Take(batchSize)
            .Select(x => new JournalRowDto
            {
                Code = x.Transnumber ?? 0,
                TypeCode = x.Transtype ?? 0,
                ReceiptNumber = x.Receiptn ?? 0,
                ProductCode = x.Productid,
                CategoryCode = x.Categoryid,
                EmploeeCode = x.Emploeeid,
                Period = x.Dater ?? default,
                Quantity = x.Quantity ?? 0,
                Price = x.Price ?? 0,
                Discount = x.Discountamount ?? 0,
                EmploeeName = x.EmploeeName,
                CategoryName = x.CategoryName,
                ProductName = x.ProductName 
            })
            .ToListAsync(token);
    }
}