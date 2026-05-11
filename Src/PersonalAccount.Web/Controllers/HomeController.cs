using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Domain.Models;
using PersonalAccount.Web.Models;
using PersonalAccount.Web.Services;

namespace PersonalAccount.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBranchSettingsService _service;

    public HomeController(IBranchSettingsService service)
    {
        _service = service;
    }

    /// <summary>
    /// Настройки. При выборе филиала загружаются его настройки.
    /// </summary>
    public IActionResult Index(Guid? branchId)
    {
        var branches = _service.GetBranches().ToList();
        var selected = branchId.HasValue ? _service.GetBranch(branchId.Value) : branches.FirstOrDefault();

        return View(new BranchSettingsModel
        {
            Branches = branches,
            Branch = selected!
        });
    }

    public IActionResult SallingReport() => View();
    public IActionResult RevenueReport() => View();
    public IActionResult WorkScheduleReport() => View();

    /// <summary>
    /// Сохранить настройки выбранного филиала.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveSettings(BranchSettingsModel model)
    {
        var branches = _service.GetBranches().ToList();
        model.Branches = branches;

        var stored = _service.GetBranch(model.Branch.Id);
        if (stored is null)
        {
            model.ErrorText = "Филиал не найден.";
            return View(nameof(Index), model);
        }

        // Восстанавливаем связи (не приходят с формы), затем валидируем по правилам домена.
        model.Branch.Name = stored.Name;
        model.Branch.Owner = stored.Owner;
        model.Branch.Settings.Branch = stored.Settings.Branch;
        model.Branch.Settings.Id = stored.Settings.Id;

        if (!model.Branch.Validate())
        {
            model.ErrorText = model.Branch.ErrorText;
            return View(nameof(Index), model);
        }

        _service.SaveSettings(model.Branch.Id, model.Branch.Settings);
        model.SuccessText = "Настройки успешно сохранены.";
        model.Branch = _service.GetBranch(model.Branch.Id)!;
        return View(nameof(Index), model);
    }
}
