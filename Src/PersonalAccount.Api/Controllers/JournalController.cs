using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JournalController : ControllerBase
{
    private readonly ILoadingService _loadingService;

    public JournalController(ILoadingService loadingService)
    {
        _loadingService = loadingService;
    }

    /// <summary>
    /// Метод для приема сырых данных (чеков) от клиента.
    /// URL: POST http://localhost:8000/api/journal/push/{companyId}
    /// </summary>
    /// <param name="branchId">Уникальный ID организации (передается в URL)</param>
    /// <param name="transactions">Список транзакций (передается в теле запроса - JSON)</param>
    /// <param name="token">Токен отмены (если клиент разорвет соединение)</param>
    [HttpPost("push/{branchId:guid}")]
    public async Task<IActionResult> PushTransactions(
        [FromRoute] Guid branchId,
        [FromBody] List<JournalRowDto> transactions,
        CancellationToken token)
    {
        if (transactions == null || !transactions.Any())
        {
            return BadRequest(new { Message = "Список транзакций пуст." });
        }

        try
        {
            var branch = new BranchModel { Id = branchId, Name = "Текущий филиал" };
            bool isSuccess = await _loadingService.PushAsync(branch, transactions, token);

            if (isSuccess)
            {
                return Ok(new { Message = $"Успешно загружено {transactions.Count} транзакций." }); 
            }
            else
            {
                return BadRequest(new { Message = "Данные не были загружены (возможно, нет новых транзакций)." });
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Ошибка в JournalController: {ex.Message}");
            return StatusCode(500, new { Message = "Произошла внутренняя ошибка сервера при сохранении данных." });
        }
    }
}