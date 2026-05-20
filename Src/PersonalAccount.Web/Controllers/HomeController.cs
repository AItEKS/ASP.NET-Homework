using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Common.Core;
using PersonalAccount.Web.Models;

namespace PersonalAccount.Web.Controllers;

public class HomeController(IBranchRepository branchRepository) : Controller
{
    // Репозиторий для работы с филиалами
    private readonly IBranchRepository _branchRepository = branchRepository;

    /// <summary>
    /// Настройки
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        var branches = _branchRepository.GetBranches().ToList();
        var branch = branches.First();

        var viewModel = new BranchSettingsViewModel()
        {
            Branches = branches,
            BranchId = branch.Id,
            Name = branch.Name,
            StartPosition = branch.Settings.StartPosition,
            BatchSize = branch.Settings.BatchSize
        };
        return View(viewModel);
    }

    /// <summary>
    /// Продажи
    /// </summary>
    /// <returns></returns>
    public IActionResult SallingReport()
    {
        return View();
    }

    /// <summary>
    /// Выручка
    /// </summary>
    /// <returns></returns>
    public IActionResult RevenueReport()
    {
        return View();
    }

    /// <summary>
    /// График работы
    /// </summary>
    /// <returns></returns>
    public IActionResult WorkScheduleReport()
    {
        return View();
    }

    /// <summary>
    /// Сохранить настройки
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult SaveSettings(BranchSettingsViewModel model)
    {
        var branch = _branchRepository.GetBranch(model.BranchId);
        branch.Name = model.Name;
        branch.Settings.StartPosition = model.StartPosition;
        branch.Settings.BatchSize = model.BatchSize;

        _branchRepository.Update(branch);

        // Повторно перегружаю Index представление
        var branches = _branchRepository.GetBranches().ToList();
        var viewModel = new BranchSettingsViewModel()
        {
            Branches = branches,
            BranchId = branch.Id,
            Name = branch.Name,
            StartPosition = branch.Settings.StartPosition,
            BatchSize = branch.Settings.BatchSize
        };
        return View("Index", viewModel);
    }
}
