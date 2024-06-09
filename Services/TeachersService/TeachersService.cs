using BookingPhongHoc.Dtos;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BookingPhongHoc;
using System.ComponentModel.DataAnnotations;
using Flurl.Http;

public class TeachersService : AirtableBaseService
{
    private readonly PasswordHasher<Teachers> _passwordHasher;

    public TeachersService(IHttpClientFactory httpClientFactory, IConfiguration configuration, PasswordHasher<Teachers> passwordHasher)
        : base(httpClientFactory, configuration, configuration["Airtable:Tables:Teachers"])
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<TeachersData> GetAllTeachersAsync()
    {
        var url = GetUrl();
        var response = await SendAsync(HttpMethod.Get, url);
        var responseContent = await response.Content.ReadAsStringAsync();
        var teachersData = JsonConvert.DeserializeObject<TeachersData>(responseContent);
        return teachersData;
    }

    public async Task<Teachers> CreateTeacherAsync(Teachers input)
    {
        // Set the default role if it's not provided
        if (input.Role == null)
        {
            input.Role = Enums.Role.gvtg;
        }

        // Hash the password before sending the request
        input.Password = _passwordHasher.HashPassword(input, input.Password);

        var record = new { records = new[] { new { fields = input } } };
        var json = JsonConvert.SerializeObject(record);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var url = GetUrl();

        var response = await SendAsync(HttpMethod.Post, url, data);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error creating teacher: {response.StatusCode}");
        }

        // Parse the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var createTeacher = JsonConvert.DeserializeObject<TeachersData>(responseContent);

        // Assuming that the first record in the response is the created teacher
        var newTeacher = createTeacher.Records[0].Fields;

        // Return a new Teachers object without the password
        return new Teachers
        {
            TeacherName = newTeacher.TeacherName,
            PhoneNumber = newTeacher.PhoneNumber,
            Email = newTeacher.Email,
            Role = newTeacher.Role,
            Avatar = newTeacher.Avatar
        };
    }

    public async Task<Teachers> UpdateTeacherAsync(string id, Teachers teacher)
    {
        var record = new { records = new[] { new { fields = teacher } } };
        var json = JsonConvert.SerializeObject(record);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var url = GetUrl(id);

        var response = await SendAsync(HttpMethod.Put, url, data);
        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedTeacherData = JsonConvert.DeserializeObject<TeachersData>(responseContent);

        // Assuming that the first record in the response is the updated teacher
        return updatedTeacherData.Records[0].Fields;
    }

    public async Task DeleteTeacherAsync(string id)
    {
        var url = GetUrl(id);
        await SendAsync(HttpMethod.Delete, url);
    }
}
