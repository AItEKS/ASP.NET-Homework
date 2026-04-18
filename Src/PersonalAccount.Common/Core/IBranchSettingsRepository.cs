using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

public interface IBranchSettingsRepository
{
    Task<LoadingSettingsModel> LoadAsync(BranchModel branch, CancellationToken token);
    Task SaveAsync(LoadingSettingsModel setting, CancellationToken token);
}