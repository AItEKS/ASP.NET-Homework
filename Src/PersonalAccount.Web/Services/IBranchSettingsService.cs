using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Services;

/// <summary>
/// Сервис доступа к филиалам и их настройкам загрузки.
/// </summary>
public interface IBranchSettingsService
{
    IReadOnlyList<BranchModel> GetBranches();
    BranchModel? GetBranch(Guid id);
    void SaveSettings(Guid branchId, LoadingSettingsModel settings);
}
