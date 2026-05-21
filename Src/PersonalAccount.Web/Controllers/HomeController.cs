using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models.Dto;
using PersonalAccount.Web.Models;

namespace PersonalAccount.Web.Controllers;

public class HomeController(IBranchRepository branchRepository, IReportService reportService) : Controller
{
    private readonly IBranchRepository _branchRepository = branchRepository;
    private readonly IReportService _reportService = reportService;

    /// <summary>
    /// Настройки
    /// </summary>
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
    [HttpGet]
    public IActionResult SallingReport()
    {
        var branches = _branchRepository.GetBranches().ToList();
        var model = new SallingViewModel
        {
            Branches = branches,
            BranchId = branches.First().Id
        };
        return View(model);
    }

    /// <summary>
    /// Продажи (отчет)
    /// </summary>
    [HttpPost]
    public IActionResult SallingReport(SallingViewModel model)
    {
        model.Branches = _branchRepository.GetBranches().ToList();
        var transactions = _reportService.Get(model.BranchId, model.Start, model.Stop);
        model.Rows = _reportService
            .Create<SellingDto>(transactions, ReportTypeEnum.Salling)
            .ToList();
        return View(model);
    }

    /// <summary>
    /// Выручка
    /// </summary>
    [HttpGet]
    public IActionResult RevenueReport()
    {
        var branches = _branchRepository.GetBranches().ToList();
        var model = new RevenueViewModel
        {
            Branches = branches,
            BranchId = branches.First().Id
        };
        return View(model);
    }

    /// <summary>
    /// Выручка (отчет)
    /// </summary>
    [HttpPost]
    public IActionResult RevenueReport(RevenueViewModel model)
    {
        model.Branches = _branchRepository.GetBranches().ToList();
        var transactions = _reportService.Get(model.BranchId, model.Start, model.Stop);
        model.Rows = _reportService
            .Create<RevenueDto>(transactions, ReportTypeEnum.Revenue)
            .ToList();
        return View(model);
    }

    /// <summary>
    /// График работы
    /// </summary>
    [HttpGet]
    public IActionResult WorkScheduleReport()
    {
        var branches = _branchRepository.GetBranches().ToList();
        var model = new WorkScheduleViewModel
        {
            Branches = branches,
            BranchId = branches.First().Id
        };
        return View(model);
    }

    /// <summary>
    /// График работы (отчет)
    /// </summary>
    [HttpPost]
    public IActionResult WorkScheduleReport(WorkScheduleViewModel model)
    {
        model.Branches = _branchRepository.GetBranches().ToList();
        var transactions = _reportService.Get(model.BranchId, model.Start, model.Stop);
        model.Rows = _reportService
            .Create<WorkScheduleDto>(transactions, ReportTypeEnum.WorkSchedule)
            .ToList();
        return View(model);
    }

    /// <summary>
    /// Сохранить настройки
    /// </summary>
    [HttpPost]
    public IActionResult SaveSettings(BranchSettingsViewModel model)
    {
        var branch = _branchRepository.GetBranch(model.BranchId);
        branch.Name = model.Name;
        branch.Settings.StartPosition = model.StartPosition;
        branch.Settings.BatchSize = model.BatchSize;

        _branchRepository.Update(branch);

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
