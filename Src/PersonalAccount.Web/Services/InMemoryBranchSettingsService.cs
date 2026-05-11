using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Services;

/// <summary>
/// Хранение филиалов и настроек загрузки в памяти процесса.
/// </summary>
public class InMemoryBranchSettingsService : IBranchSettingsService
{
    private readonly List<BranchModel> _branches;

    public InMemoryBranchSettingsService()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "ООО \"Ромашка\"",
            INN = "7701234567",
            Address = "Россия,Москва,Москва,Тверская,12,1"
        };

        _branches = new List<BranchModel>
        {
            CreateBranch(company, "Филиал \"Центральный\"", 0, 1000),
            CreateBranch(company, "Филиал \"Северный\"", 5000, 500),
            CreateBranch(company, "Филиал \"Южный\"", 12000, 2000),
        };
    }

    private static BranchModel CreateBranch(CompanyModel company, string name, long start, long batch)
    {
        var branchId = Guid.NewGuid();
        return new BranchModel
        {
            Id = branchId,
            Name = name,
            Owner = company,
            Settings = new LoadingSettingsModel
            {
                Id = Guid.NewGuid(),
                Branch = new BranchModel { Id = branchId, Name = name, Owner = company },
                Description = $"Настройки загрузки для {name}",
                StartPosition = start,
                BatchSize = batch
            }
        };
    }

    public IReadOnlyList<BranchModel> GetBranches() => _branches;

    public BranchModel? GetBranch(Guid id) => _branches.FirstOrDefault(b => b.Id == id);

    public void SaveSettings(Guid branchId, LoadingSettingsModel settings)
    {
        var branch = GetBranch(branchId)
            ?? throw new InvalidOperationException($"Филиал {branchId} не найден.");

        branch.Settings.Description = settings.Description;
        branch.Settings.StartPosition = settings.StartPosition;
        branch.Settings.BatchSize = settings.BatchSize;
    }
}