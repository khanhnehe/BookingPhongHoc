using Microsoft.AspNetCore.Mvc;
using BookingPhongHoc.Dtos;
using Newtonsoft.Json;

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
            var json = await _teachersService.GetAllTeachersAsync();
            var teachersData = JsonConvert.DeserializeObject<TeachersData>(json);
            var teachersList = teachersData.Records.Select(record => record.Fields).ToList();
            var teachersJson = JsonConvert.SerializeObject(teachersList, Formatting.Indented);
            return Ok(teachersJson);
        }

        [HttpPost("add-teacher")]
        public async Task<IActionResult> AddTeacher(Teachers teacher)
        {
            var result = await _teachersService.CreateTeacherAsync(teacher);
            return Ok(result);
        }

        [HttpPut("update-teacher/{id}")]
        public async Task<IActionResult> UpdateTeacher(string id, Teachers teacher)
        {
            var result = await _teachersService.UpdateTeacherAsync(id, teacher);
            return Ok(result);
        }

        [HttpDelete("delete-teacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            await _teachersService.DeleteTeacherAsync(id);
            return Ok();
        }
    }
}
