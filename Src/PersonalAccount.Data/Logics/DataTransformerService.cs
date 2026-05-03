using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Data.Logics;

public class DataTransformerService : IDataTransformerService
{
    public IEnumerable<TransactionModel> MapToDomain(
        BranchModel branch,
        IEnumerable<JournalRowDto> rows,
        IEnumerable<EmploeeModel> employees,
        IEnumerable<NomenclatureModel> nomenclature)
    {
        var empMap = employees.ToDictionary(x => x.Code);
        var nomMap = nomenclature.ToDictionary(x => x.Code);

        var result = new List<TransactionModel>();

        foreach (var row in rows)
        {
            if (row.EmploeeCode.HasValue && empMap.TryGetValue(row.EmploeeCode.Value, out var emploee) &&
                row.ProductCode.HasValue && nomMap.TryGetValue(row.ProductCode.Value, out var product))
            {
                result.Add(new TransactionModel
                {
                    Id = Guid.NewGuid(),
                    Branch = branch,
                    Owner = branch.Owner,
                    
                    Period = row.Period,
                    Quantuty = row.Quantity,
                    Price = row.Price,
                    Discount = row.Discount,
                    TicketNumber = row.ReceiptNumber.ToString(),
                    
                    Emploee = emploee,
                    Nomenclature = product,
                    
                    Type = (TransactionType)row.TypeCode
                });
            }
        }

        return result;
    }
}