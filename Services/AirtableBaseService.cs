using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class AirtableBaseService
{
    protected readonly HttpClient _httpClient;
    protected readonly string _apiKey;
    protected readonly string _baseId;
    protected readonly string _tableId;
    private const string BaseUrl = "https://api.airtable.com/v0";

    public AirtableBaseService(HttpClient httpClient, IConfiguration configuration, string tableId)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Airtable:ApiKey"];
        _baseId = configuration["Airtable:BaseId"];
        _tableId = tableId;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GetAllRecordsAsync()
    {
        var requestUrl = $"{BaseUrl}/{_baseId}/{_tableId}";
        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        throw new Exception($"Error fetching records: {response.StatusCode}");
    }

}
