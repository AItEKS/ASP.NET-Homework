using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Data.Logics;

/// <summary>
/// Реализация репозитория для работы с настройками филиалов.
/// </summary>
public class BranchSettingsRepository : IBranchSettingsRepository
{
    private readonly PersonalAccountContext _context;

    public BranchSettingsRepository(PersonalAccountContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Загрузить настройки филиала.
    /// </summary>
    public LoadingSettingsModel? Load(BranchModel branch)
    {
        var item = _context.Branches.FirstOrDefault(x => x.Id == branch.Id)
            ?? throw new InvalidDataException($"Не найден филиал по коду {branch.Id}!");

        if (string.IsNullOrEmpty(item.LoadOptions))
            return null;

        var result = JsonSerializer.Deserialize<LoadingSettingsModel>(item.LoadOptions)
            ?? throw new InvalidDataException($"Ошибка десериализации настроек филиала {branch.Id}");

        result.Owner = branch;
        return result;
    }

    /// <summary>
    /// Сохранить настройки филиала.
    /// </summary>
    public void Save(LoadingSettingsModel settings)
    {
        var branchId = settings.Owner?.Id 
            ?? throw new InvalidDataException("Невозможно сохранить настройки: нет информации о филиале!");

        var branchEntity = _context.Branches.FirstOrDefault(x => x.Id == branchId)
            ?? throw new InvalidDataException($"Не найден филиал по коду {branchId}!");

        branchEntity.LoadOptions = JsonSerializer.Serialize(settings);
        
        _context.SaveChanges();
    }
}