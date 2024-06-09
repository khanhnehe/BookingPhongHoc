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

    // Constructor của TeachersService, khởi tạo httpClientFactory, configuration, và passwordHasher
    public TeachersService(IHttpClientFactory httpClientFactory, IConfiguration configuration, PasswordHasher<Teachers> passwordHasher)
        : base(httpClientFactory, configuration, configuration["Airtable:Tables:Teachers"]) // Gọi constructor của lớp cơ sở AirtableBaseService
    {
        _passwordHasher = passwordHasher; 
    }


    public async Task<bool> PhoneNumberExist(string phoneNumber)
    {
        var teachersData = await GetAllTeachersAsync();
        foreach (var teacher in teachersData.Records)
        {
            if (teacher.Fields.PhoneNumber == phoneNumber)
            {
                return true;
            }
        }
        return false;
    }
    public async Task<TeachersData> GetAllTeachersAsync()
    {
        var url = GetUrl(); 
        var response = await SendAsync(HttpMethod.Get, url); 
        var responseContent = await response.Content.ReadAsStringAsync(); // Đọc nội dung phản hồi dưới dạng chuỗi
        var teachersData = JsonConvert.DeserializeObject<TeachersData>(responseContent); // Chuyển đổi chuỗi JSON thành đối tượng TeachersData
        return teachersData; 
    }

    public async Task<Teachers> CreateTeacherAsync(Teachers input)
    {

        if (await PhoneNumberExist(input.PhoneNumber))
        {
            throw new Exception("Số điện thoại đã tồn tại");
        }

        if (input.Role == null)
        {
            input.Role = Enums.Role.gvtg; 
        }


        // Mã hóa mật khẩu trước khi gửi yêu cầu
        input.Password = _passwordHasher.HashPassword(input, input.Password);

        var record = new { records = new[] { new { fields = input } } }; // Tạo object record chứa dữ liệu giáo viên
        var url = GetUrl(); // Lấy URL từ lớp cơ sở

        var response = await SendJsonAsync(HttpMethod.Post, url, record);

        var responseContent = await response.Content.ReadAsStringAsync(); 
        var createTeacher = JsonConvert.DeserializeObject<TeachersData>(responseContent); 

        var newTeacher = createTeacher.Records[0].Fields;

        return new Teachers
        {
            TeacherName = newTeacher.TeacherName,
            PhoneNumber = newTeacher.PhoneNumber,
            Email = newTeacher.Email,
            Role = newTeacher.Role,
            IsActive = newTeacher.IsActive,
            Avatar = newTeacher.Avatar
        };
    }

    public async Task<Teachers> UpdateTeacherAsync(string id, Teachers teacher)
    {
        var record = new { records = new[] { new { fields = teacher } } }; // Tạo object record chứa dữ liệu giáo viên cần cập nhật
        var url = GetUrl(id); // Lấy URL từ lớp cơ sở với id của giáo viên

        // Gửi yêu cầu PUT để cập nhật giáo viên
        var response = await SendJsonAsync(HttpMethod.Put, url, record);
        var responseContent = await response.Content.ReadAsStringAsync(); // Đọc nội dung phản hồi dưới dạng chuỗi
        var updatedTeacherData = JsonConvert.DeserializeObject<TeachersData>(responseContent); // Chuyển đổi chuỗi JSON thành đối tượng TeachersData

        // Giả sử rằng bản ghi đầu tiên trong phản hồi là giáo viên vừa được cập nhật
        return updatedTeacherData.Records[0].Fields;
    }

    // Phương thức xóa giáo viên
    public async Task DeleteTeacherAsync(string id)
    {
        var url = GetUrl(id); // Lấy URL từ lớp cơ sở với id của giáo viên
        await SendAsync(HttpMethod.Delete, url); // Gửi yêu cầu HTTP DELETE đến URL để xóa giáo viên
    }
}
