using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

public interface IBusinessDataRepository
{
    Task SaveCategoriesAsync(IEnumerable<CategoryModel> categories, CancellationToken token);
    Task SaveNomenclatureAsync(IEnumerable<NomenclatureModel> nomenclature, CancellationToken token);
    Task SaveEmployeesAsync(IEnumerable<EmploeeModel> employees, CancellationToken token);
    Task SaveTransactionsAsync(IEnumerable<TransactionModel> transactions, CancellationToken token);
}