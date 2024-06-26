﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class AirtableBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    protected readonly string _apiKey;
    protected readonly string _baseId;
    protected readonly string _tableId;
    protected const string BaseUrl = "https://api.airtable.com/v0";

    public AirtableBaseService(IHttpClientFactory httpClientFactory, IConfiguration configuration, string tableId)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["Airtable:ApiKey"];
        _baseId = configuration["Airtable:BaseId"];
        _tableId = tableId;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        return client;
    }

    public string GetUrl(string id = null)
    {
        return id == null ? $"{BaseUrl}/{_baseId}/{_tableId}" : $"{BaseUrl}/{_baseId}/{_tableId}/{id}";
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content = null)
    {
        var client = CreateClient();
        var request = new HttpRequestMessage(method, url) { Content = content };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error {method} record: {response.StatusCode}\n{responseContent}");
        }

        return response;
    }

    public async Task<HttpResponseMessage> SendJsonAsync(HttpMethod method, string url, object record)
    {
        var json = JsonConvert.SerializeObject(record);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        return await SendAsync(method, url, data);
    }
}
