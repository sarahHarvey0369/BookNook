using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookNook.ReadingStatisticsService.DTOs;

namespace BookNook.ReadingStatisticsService.Services;

public class LibraryServiceClient(HttpClient httpClient)
{
    public async Task<List<LibraryBookDto>> GetLibraryAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/library");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<LibraryBookDto>>() ?? [];
    }
}