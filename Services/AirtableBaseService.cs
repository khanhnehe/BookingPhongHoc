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
    protected const string BaseUrl = "https://api.airtable.com/v0";

    public AirtableBaseService(HttpClient httpClient, IConfiguration configuration, string tableId)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Airtable:ApiKey"];
        _baseId = configuration["Airtable:BaseId"];
        _tableId = tableId;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    protected string GetUrl(string id = null)
    {
        return id == null ? $"{BaseUrl}/{_baseId}/{_tableId}" : $"{BaseUrl}/{_baseId}/{_tableId}/{id}";
    }

    protected async Task<string> SendAsync(HttpMethod method, string url, HttpContent content = null)
    {
        var request = new HttpRequestMessage(method, url) { Content = content };
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw new Exception($"Error {method} record: {response.StatusCode}");
    }
}
