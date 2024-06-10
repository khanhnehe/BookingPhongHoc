using AutoWrapper.Wrappers;
using BookingPhongHoc.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BookingPhongHoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeachersService _teachersService;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(TeachersService teachersService, ILogger<TeachersController> logger)
        {
            _teachersService = teachersService;
            _logger = logger;
        }
        [Authorize]
        [HttpGet("get-all-teachers")]
        public async Task<object> GetAllTeachers()
        {
            try
            {
                var teachersData = await _teachersService.GetAllTeachers();
                var Getteachers = teachersData.Records.Select(record => new Teachers
                {
                    TeacherName = record.Fields.TeacherName,
                    PhoneNumber = record.Fields.PhoneNumber,
                    Email = record.Fields.Email,
                    Role = record.Fields.Role,
                    IsActive = record.Fields.IsActive,
                    Avatar = record.Fields.Avatar
                }).ToArray();
                return new { teachers = Getteachers };
            }
            catch (ApiException ex)
            {
                throw new ApiException($"{ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiException($"Có lỗi xảy ra: {ex.Message}");
            }
        }


        [Authorize]
        [HttpPost("create-teacher")]
        public async Task<object> CreateTeacher([FromBody] Teachers teacher)
        {
            try
            {
                var newTeacher = await _teachersService.CreateTeacher(teacher);
                return new { newTeacher };
            }
            catch (ApiException ex)
            {
                throw new ApiException($"{ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiException($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<ApiResponse> SignIn([FromBody] SignIn signInRequest)
        {
            try
            {
                var token = await _teachersService.SignIn(signInRequest);
                return new ApiResponse("Đăng nhập thành công", token);
            }
            catch (ApiException ex)
            {
                throw new ApiException($"{ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiException($"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
