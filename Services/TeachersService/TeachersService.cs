using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BookingPhongHoc.Dtos;

public class TeachersService : AirtableBaseService
{
    public TeachersService(HttpClient httpClient, IConfiguration configuration)
        : base(httpClient, configuration, configuration["Airtable:Tables:Teachers"])
    {
    }

    public async Task<string> GetAllTeachersAsync()
    {
        var url = GetUrl();
        return await SendAsync(HttpMethod.Get, url);
    }

    public async Task<string> CreateTeacherAsync(Teachers teacher)
    {
        var teacherFields = new TeachersFields { Fields = teacher };
        var json = JsonConvert.SerializeObject(teacherFields);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var url = GetUrl();

        return await SendAsync(HttpMethod.Post, url, data);
    }

    public async Task<string> UpdateTeacherAsync(string id, Teachers teacher)
    {
        var teacherFields = new TeachersFields { Fields = teacher };
        var json = JsonConvert.SerializeObject(teacherFields);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var url = GetUrl(id);

        return await SendAsync(HttpMethod.Put, url, data);
    }

    public async Task DeleteTeacherAsync(string id)
    {
        var url = GetUrl(id);
        await SendAsync(HttpMethod.Delete, url);
    }
}
