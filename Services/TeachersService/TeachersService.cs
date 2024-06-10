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
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

public class TeachersService : AirtableBaseService
{
    private readonly PasswordHasher<Teachers> _passwordHasher;
    private readonly IConfiguration _config;  // Thêm biến này

    // Constructor của TeachersService, khởi tạo httpClientFactory, configuration, và passwordHasher
    public TeachersService(IHttpClientFactory httpClientFactory, IConfiguration configuration, PasswordHasher<Teachers> passwordHasher)
        : base(httpClientFactory, configuration, configuration["Airtable:Tables:Teachers"]) // Gọi constructor của lớp cơ sở AirtableBaseService
    {
        _passwordHasher = passwordHasher;
        _config = configuration;  // Khởi tạo biến này
    }

    // Các phương thức khác của TeachersService
    public async Task<bool> PhoneNumberExist(string phoneNumber)
    {
        var teachersData = await GetAllTeachers();
        foreach (var teacher in teachersData.Records)
        {
            if (teacher.Fields.PhoneNumber == phoneNumber)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<TeachersData> GetAllTeachers()
    {
        var url = GetUrl();
        var response = await SendAsync(HttpMethod.Get, url);
        var responseContent = await response.Content.ReadAsStringAsync(); // Đọc nội dung phản hồi dưới dạng chuỗi
        var teachersData = JsonConvert.DeserializeObject<TeachersData>(responseContent); // Chuyển đổi chuỗi JSON thành đối tượng TeachersData
        return teachersData;
    }

    public async Task<Teachers> CreateTeacher(Teachers input)
    {
        if (input.PhoneNumber.Length != 10)
        {
            throw new Exception("Vui lòng nhập đúng số điện thoại");
        }

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

    public async Task<string> SignIn(SignIn input)
    {
        var teachersData = await GetAllTeachers();
        foreach (var teacher in teachersData.Records)
        {
            if (teacher.Fields.PhoneNumber == input.PhoneNumber && teacher.Fields.IsActive)
            {
                // Kiểm tra mật khẩu
                var result = _passwordHasher.VerifyHashedPassword(teacher.Fields, teacher.Fields.Password, input.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    // Tạo JWT nếu mật khẩu đúng
                    return GenerateJWT(teacher.Fields);
                }
                else
                {
                    throw new Exception("Mật khẩu không đúng");
                }
            }
        }
        throw new Exception("Không tìm thấy giáo viên với số điện thoại này");
    }

    private string GenerateJWT(Teachers teacher)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.MobilePhone, teacher.PhoneNumber),
            new Claim(ClaimTypes.Role, teacher.Role.ToString()),
            new Claim("Role", teacher.Role?.ToString()),
            new Claim("TeacherName", teacher.TeacherName),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.UtcNow.AddHours(7).AddDays(30),
            signingCredentials: credentials,
            claims: claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var stringToken = tokenHandler.WriteToken(token);
        return stringToken;
    }
}
