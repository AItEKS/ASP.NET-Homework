using System;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Реализация интерфейса <see cref="ISalesReportService"/>
/// </summary>
public class SalesReportService : ISalesReportService
{
    /// <summary>
    /// Сформировать отчет
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <returns></returns>
    public IEnumerable<SellingDto> Create(IEnumerable<TransactionModel> transactions)
    {
        if(!transactions.Any()) return  Enumerable.Empty<SellingDto>() ;
        var companyId = transactions.FirstOrDefault()?.Owner.Id ?? throw new InvalidOperationException("Невозможно получить код корганизации!");
        
        var items = transactions.Where(x => x.Type == Domain.Core.TransactionType.Sale);
        var result = items.Select( x => new SellingDto()
        {
            Period = x.Period.Date,
            CategoryId = x.Nomenclature.Category.Id,
            CategoryName = x.Nomenclature.Category.Name,
            NomenclatureId = x.Nomenclature.Id,
            NomenclatureName = x.Nomenclature.Name,
            Quantuty = x.Quantuty,
            Amount = (x.Quantuty * x.Price) - x.Discount,
            DiscountAmount = x.Discount,
            CompanyId =  companyId
        }).OrderBy( x => x.Period);

        return result;
    }

    /// <summary>
    /// Реализация ассинхронного варианта
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SellingDto>> CreateAsync(IEnumerable<TransactionModel> transactions, CancellationToken token)
        => await Task.Run( () => Create( transactions), token);
}
