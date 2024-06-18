using AutoWrapper.Wrappers;
using BookingPhongHoc.Dtos;
using BookingPhongHoc.Repositories;
using BookingPhongHoc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingPhongHoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeachersService _teachersService;
        private readonly ILogger<TeachersController> _logger;
        //private readonly ITeachersRepository _teachersRepository;

        public TeachersController(TeachersService teachersService, ILogger<TeachersController> logger)
        {
            _teachersService = teachersService;
            _logger = logger;
            //_teachersRepository = teachersRepository;
        }

        //[HttpGet("GetTeacherRole/{teacherId}")]
        //public async Task<IActionResult> GetTeacherRole(string teacherId)
        //{
        //    var role = await _teachersRepository.GetTeacherRoleById(teacherId);

        //    if (role.HasValue)
        //    {
        //        return Ok(role.Value);
        //    }
        //    else
        //    {
        //        return NotFound("Không tìm thấy vai trò cho giáo viên này.");
        //    }
        //}

        [Authorize]
        [HttpGet("get-all-teachers")]
        public async Task<object> GetAllTeachers()
        {
            try
            {
                var teachersData = await _teachersService.GetAllTeachers();
                var teachers = teachersData.Records.Select(record => new
                {
                    Id = record.Id,
                    TeacherName = record.Fields.TeacherName,
                    PhoneNumber = record.Fields.PhoneNumber,
                    Email = record.Fields.Email,
                    Role = record.Fields.Role,
                    IsActive = record.Fields.IsActive,
                    Avatar = record.Fields.Avatar
                }).ToArray();
                return new { teachers };
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

        [HttpGet("Role/{teacherId}")]
        public async Task<IActionResult> GetTeacherRoleById(string teacherId)
        {
            try
            {
                var role = await _teachersService.GetTeacherRoleById(teacherId);
                if (role != null)
                {
                    return Ok(role);
                }
                else
                {
                    return NotFound($"Không tìm thấy Role cho giáo viên với ID: {teacherId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        [Authorize]
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePassword changePassword)
        {
            try
            {
                await _teachersService.ChangePassword(id, changePassword.PhoneNumber, changePassword.CurrentPassword, changePassword.NewPassword);
                return Ok(new { message = "Password changed successfully" });
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
        [HttpPut("update-teacher/{id}")]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] Teachers input)
        {
            try
            {
                var updatedTeacher = await _teachersService.UpdateTeacher(id, input);
                return Ok(new { updatedTeacher });
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
        [HttpDelete("delete-teacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            try
            {
                await _teachersService.DeleteTeacher(id);
                return Ok(new { message = "Teacher deleted successfully" });
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
