using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Интерфейс репозитория для работы с настройками филиалов.
/// </summary>
public interface IBranchSettingsRepository
{
    /// <summary>
    /// Загрузить настройки филиала.
    /// </summary>
    LoadingSettingsModel? Load(BranchModel branch);

    /// <summary>
    /// Сохранить настройки филиала.
    /// </summary>
    void Save(LoadingSettingsModel settings);
}