using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Common.Core;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Data.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IBranchSettingsRepository"/>
/// </summary>
public class BranchSettingsRepository : IBranchSettingsRepository
{
    private readonly PersonalAccountContext _context;

    /// <summary>
    /// Создать объект типа <see cref="BranchSettingsRepository"/>
    /// </summary>
    /// <param name="context"> Контекст для работы с базой данных </param>
    public BranchSettingsRepository(PersonalAccountContext context) 
    {
        _context = context;
    }

    /// <summary>
    /// Загрузить настройки филиала
    /// </summary>
    public async Task<LoadingSettingsModel> LoadAsync(BranchModel branch, CancellationToken token)
    {
        var item = _context.Branches.FirstOrDefault(x => x.Id == branch.Id)
            ?? throw new InvalidDataException($"Не найден филиал по коду {branch.Id}!");
            
        var json = !string.IsNullOrEmpty(item.LoadOptions) ? item.LoadOptions
            : throw new InvalidDataException($"Филиал по коду {branch.Id} содержит некорректные данные по настройкам!");

        var result = JsonSerializer.Deserialize<LoadingSettingsModel>(json)
            ?? throw new InvalidDataException($"Филиал по коду {branch.Id} содержит некорректные данные по настройкам!");
            
        result.Owner = branch; 
        
        return result;
    }

    /// <summary>
    /// Сохранить настройки филиала
    /// </summary>
    public async Task SaveAsync(LoadingSettingsModel setting, CancellationToken token)
    {
        var branchId = setting.Owner?.Id ?? throw new InvalidDataException("Невозможно сохранить настройки т.к. нет информации о филиале!");
        
        var branchEntity = _context.Branches.FirstOrDefault(x => x.Id == branchId)
            ?? throw new InvalidDataException($"Не найден филиал по коду {branchId}!");

        var text = JsonSerializer.Serialize(setting);
        
        branchEntity.LoadOptions = text;
        
        await _context.SaveChangesAsync(token);
   }
}