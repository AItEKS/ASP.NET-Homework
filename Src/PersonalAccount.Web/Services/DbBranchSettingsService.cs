using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Services;

/// <summary>
/// Сервис работы с филиалами и настройками загрузки через базу данных.
/// </summary>
public class DbBranchSettingsService : IBranchSettingsService
{
    private readonly PersonalAccountContext _context;

    public DbBranchSettingsService(PersonalAccountContext context)
    {
        _context = context;
    }

    public IReadOnlyList<BranchModel> GetBranches()
    {
        var branches = _context.Branches
            .Include(b => b.Company)
            .AsNoTracking()
            .ToList();

        return branches.Select(MapToModel).ToList();
    }

    public BranchModel? GetBranch(Guid id)
    {
        var branch = _context.Branches
            .Include(b => b.Company)
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == id);

        return branch == null ? null : MapToModel(branch);
    }

    public void SaveSettings(Guid branchId, LoadingSettingsModel settings)
    {
        var branch = _context.Branches.FirstOrDefault(b => b.Id == branchId)
            ?? throw new InvalidOperationException($"Филиал {branchId} не найден.");

        var loadOptions = new
        {
            Description = settings.Description,
            StartPosition = settings.StartPosition,
            BatchSize = settings.BatchSize
        };

        branch.LoadOptions = JsonSerializer.Serialize(loadOptions);
        _context.SaveChanges();
    }

    private static BranchModel MapToModel(Data.Models.Branch branch)
    {
        var company = new CompanyModel
        {
            Id = branch.Company.Id,
            Name = branch.Company.Name ?? string.Empty,
            INN = branch.Company.Inn ?? string.Empty,
            Address = branch.Company.Address ?? string.Empty
        };

        var loadSettings = ParseLoadOptions(branch.LoadOptions);

        return new BranchModel
        {
            Id = branch.Id,
            Name = branch.Name ?? string.Empty,
            Owner = company,
            Settings = new LoadingSettingsModel
            {
                Id = Guid.NewGuid(),
                Branch = new BranchModel { Id = branch.Id, Name = branch.Name ?? string.Empty, Owner = company },
                Description = loadSettings.Description,
                StartPosition = loadSettings.StartPosition,
                BatchSize = loadSettings.BatchSize
            }
        };
    }

    private static (string Description, long StartPosition, long BatchSize) ParseLoadOptions(string? loadOptionsJson)
    {
        if (string.IsNullOrWhiteSpace(loadOptionsJson))
        {
            return (string.Empty, 0, 1000);
        }

        try
        {
            var options = JsonSerializer.Deserialize<JsonElement>(loadOptionsJson);

            var description = options.TryGetProperty("Description", out var desc)
                ? desc.GetString() ?? string.Empty
                : string.Empty;

            var startPosition = options.TryGetProperty("StartPosition", out var start)
                ? start.GetInt64()
                : 0;

            var batchSize = options.TryGetProperty("BatchSize", out var batch)
                ? batch.GetInt64()
                : 1000;

            return (description, startPosition, batchSize);
        }
        catch
        {
            return (string.Empty, 0, 1000);
        }
    }
}
