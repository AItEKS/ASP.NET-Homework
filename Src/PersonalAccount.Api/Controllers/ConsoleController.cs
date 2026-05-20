using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Controllers
{
    /// <summary>
    /// Контроллер для вызов Api (загрузка данных)
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConsoleController : ControllerBase
    {
        // Сервис загрузки данных
        private readonly ILoadingFromClientService _loadingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadingService"></param>
        public ConsoleController(ILoadingFromClientService loadingService) => _loadingService = loadingService;

        /// <summary>
        /// Выполнить загрузку данных в raw таблицу из журнала.
        /// </summary>
        /// <param name="companyId"> Уникальный код организации </param>
        /// <param name="transactions"> Список транзакций </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDto))]
        public async Task<ActionResult> Push(
            [FromRoute]
            Guid companyId,
            [FromBody]
            IEnumerable<JournalRowDto> transactions,
            CancellationToken token
        )
        {
            var result = await _loadingService.PushAsync(companyId, transactions, token);
            if (result) return Ok();
            else
                return BadRequest();
        }

        // Пример запроса.
        // select * from branches
        // http://0.0.0.0:8000/console/655315b0-f7dd-463b-abeb-01ba3f770cac

        /// <summary>
        /// Получить текущие настройки дяя указанной организации
        /// </summary>
        /// <param name="branchId"> Уникальный код филиала </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{branchId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoadingSettingsModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDto))]
        public async Task<ActionResult> GetSettings(
            [FromRoute]
            Guid branchId,
             CancellationToken token
        )
        {
            var result = await _loadingService.GetSettingsAsync(branchId, token);
            if (result is not null) return Ok(result);
            else
                return BadRequest();
        }
    }
}
