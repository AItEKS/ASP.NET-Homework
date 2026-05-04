using System.Net.Http.Json;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Console.Logics;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<bool> SendTransactionsAsync(Guid branchId, IEnumerable<JournalRowDto> transactions)
    {
        var response = await _httpClient.PostAsJsonAsync($"console/{branchId}", transactions);
        return response.IsSuccessStatusCode;
    }
}