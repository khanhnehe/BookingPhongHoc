using BookingPhongHoc.Dtos;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-all-teachers")]
        public async Task<object> GetAllTeachers()
        {
            try
            {
                var teachers = await _teachersService.GetAllTeachersAsync();
                return new { message = "GET Request successful.", result = teachers };
            }
            catch (ApiException ex)
            {
                throw new ApiException($"{ex.Message}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiException("Có lỗi xảy ra.");
            }
        }

        [HttpPost("create-teacher")]
        public async Task<object> CreateTeacher([FromBody] Teachers teacher)
        {
            try
            {
                var NewTeacher = await _teachersService.CreateTeacherAsync(teacher);
                return new { message = "Teacher created successfully", result = NewTeacher };
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
