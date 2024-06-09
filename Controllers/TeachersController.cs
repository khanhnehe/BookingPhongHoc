using Microsoft.AspNetCore.Mvc;
using BookingPhongHoc.Dtos;
using System.Threading.Tasks;

namespace BookingPhongHoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeachersService _teachersService;

        public TeachersController(TeachersService teachersService)
        {
            _teachersService = teachersService;
        }

        [HttpGet("get-all-teachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                var teachers = await _teachersService.GetAllTeachersAsync();
                return Ok(new { message = "GET Request successful.", result = teachers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while fetching the teachers: {ex.Message}" });
            }
        }

        [HttpPost("create-teacher")]
        public async Task<IActionResult> CreateTeacher([FromBody] Teachers teacher)
        {


            try
            {
                var NewTeacher = await _teachersService.CreateTeacherAsync(teacher);
                return Ok(new { message = "Teacher created successfully", result = NewTeacher });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while creating the teacher: {ex.Message}" });
            }
        }

        // The rest of your code...
    }
}
