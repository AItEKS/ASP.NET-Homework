using PersonalAccount.Domain.Models;

namespace PersonalAccount.Domain.Core;

public interface ILoadingSettingsRepo
{
    public Task<bool> Save(Organization organization, CancellationToken token);
    public Task<bool> Load(Organization organization, CancellationToken token);
}