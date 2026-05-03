using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Интерфейс репозитория для финальной записи обработанных бизнес-данных в базу данных.
/// </summary>
public interface IBusinessDataRepository
{
    /// <summary>
    /// Выполняет массовое сохранение новых категорий номенклатуры.
    /// </summary>
    /// <param name="categories">Коллекция моделей категорий.</param>
    Task SaveCategoriesAsync(IEnumerable<CategoryModel> categories, CancellationToken token);

    /// <summary>
    /// Выполняет массовое сохранение новых позиций номенклатуры (товаров).
    /// </summary>
    /// <param name="nomenclature">Коллекция моделей номенклатуры.</param>
    Task SaveNomenclatureAsync(IEnumerable<NomenclatureModel> nomenclature, CancellationToken token);

    /// <summary>
    /// Выполняет массовое сохранение новых сотрудников.
    /// </summary>
    /// <param name="employees">Коллекция моделей сотрудников.</param>
    Task SaveEmployeesAsync(IEnumerable<EmploeeModel> employees, CancellationToken token);
    
    /// <summary>
    /// Подготавливает базу данных к вставке новой пачки транзакций.
    /// </summary>
    Task PrepareTransactionsAsync(IEnumerable<TransactionModel> transactions, CancellationToken token);

    /// <summary>
    /// Выполняет массовое сохранение транзакций (чеков) в основную бизнес-таблицу.
    /// </summary>
    /// <param name="transactions">Коллекция моделей транзакций.</param>
    Task SaveTransactionsAsync(IEnumerable<TransactionModel> transactions, CancellationToken token);
}