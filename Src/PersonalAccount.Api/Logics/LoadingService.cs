using Microsoft.EntityFrameworkCore;
using PersonalAccount.Common.Core;
using PersonalAccount.Data;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Реализация сервиса загрузки данных.
/// </summary>
public class LoadingService : ILoadingService
{
    private readonly IBranchSettingsRepository _settingReposity;
    private readonly IServerRepository<JournalRowDto> _writerRepository;
    private readonly PersonalAccountContext _context;
    
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LoadingService"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="settingsRepository">Репозиторий настроек.</param>
    /// <param name="writerRepository">Репозиторий записи.</param>
    public LoadingService( 
            PersonalAccountContext context,
            IBranchSettingsRepository settingsRepository, 
            IServerRepository<JournalRowDto> writerRepository)
    {
        _context = context;
        _settingReposity = settingsRepository;
        _writerRepository = writerRepository;
    }        
       
    /// <summary>
    /// Внутренний метод обработки пакета данных филиала.
    /// </summary>
    private bool Push(BranchModel branch, IEnumerable<JournalRowDto> transactions)
    {
        var settings = _settingReposity.Load(branch) 
                        ?? new LoadingSettingsModel()
                        {
                            Owner = branch, StartPosition = 1, BatchSize = 1000
                        };

        var firstTransaction = transactions.FirstOrDefault();
        if(firstTransaction is null) return false;
        
        var innerTransactions = transactions.Where(x => x.Code >= settings.StartPosition).ToList();

        if (!innerTransactions.Any()) return true;

        var connect = _context.Database.GetDbConnection();
        var task = _writerRepository.SaveRows(connect, innerTransactions, settings);
        
        var lastCode = innerTransactions.OrderByDescending(x => x.Code).First().Code;
        settings.StartPosition = lastCode + 1;
        _settingReposity.Save(settings);

        Task.WaitAll(task);
        return true;
    }

    /// <summary>
    /// Загрузить пакет данных по коду филиала.
    /// </summary>
    public bool Push(Guid branchId, IEnumerable<JournalRowDto> transactions)
    {
        var branch = _context.Branches
            .Include(x => x.Company)
            .FirstOrDefault(x => x.Id == branchId) 
            ?? throw new InvalidOperationException($"Невозможно получить карточку филиала по коду {branchId}!");

        var branchModel = new BranchModel() 
        { 
            Id = branch.Id, 
            Name = branch.Name,
            Owner = new CompanyModel() 
            { 
                Id = branch.Company.Id,
                Name = branch.Company.Name ?? string.Empty,
                INN = branch.Company.Inn ?? string.Empty,
                Address = branch.Company.Address ?? string.Empty
            }
        };

        return Push(branchModel, transactions);
    }

    /// <summary>
    /// Асинхронная загрузка данных.
    /// </summary>
    public async Task<bool> PushAsync(Guid branchId, IEnumerable<JournalRowDto> transactions, CancellationToken token)
        => await Task.Run(() => Push(branchId, transactions), token);

    /// <summary>
    /// Получить настройки по коду филиала.
    /// </summary>
    public LoadingSettingsModel GetSettings(Guid branchId)
    {
        var branch = _context.Branches
            .Include(x => x.Company)
            .FirstOrDefault(x => x.Id == branchId) 
            ?? throw new InvalidOperationException($"Невозможно получить карточку филиала по коду {branchId}!");
        
        var branchModel = new BranchModel()
        { 
            Id = branch.Id, 
            Name = branch.Name,
            Owner = new CompanyModel()
            {
                Id = branch.Company.Id,
                Name = branch.Company.Name ?? string.Empty,
                INN = branch.Company.Inn ?? string.Empty,
                Address = branch.Company.Address ?? string.Empty
            }
        };

        var settings = _settingReposity.Load(branchModel);

        if (settings is null)
        {
            settings = new LoadingSettingsModel()
                        {
                            Owner = branchModel, StartPosition = 1, BatchSize = 1000
                        };
            _settingReposity.Save(settings);            
        }

        return settings;
    }

    /// <summary>
    /// Асинхронное получение настроек.
    /// </summary>
    public async Task<LoadingSettingsModel> GetSettingsAsync(Guid branchId, CancellationToken token)
    {
        var result = await Task.Run(() => GetSettings(branchId), token);
        return result;
    }
}