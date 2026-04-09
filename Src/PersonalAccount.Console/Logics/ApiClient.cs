using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Console.Logics;

/// <summary>
/// Сервис для отправки данных на Web API
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendTransactionsAsync(Guid companyId, IEnumerable<JournalRowDto> transactions)
    {
        try
        {
            string url = $"api/journal/push/{companyId}";

            var response = await _httpClient.PostAsJsonAsync(url, transactions);

            if (response.IsSuccessStatusCode)
            {
                System.Console.WriteLine($"[OK] Успешно отправлено {transactions.Count()} чеков.");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine($"[ERROR] Ошибка сервера ({response.StatusCode}): {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"[CRITICAL] Ошибка связи с сервером: {ex.Message}");
            return false;
        }
    }
}