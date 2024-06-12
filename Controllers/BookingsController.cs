using AutoWrapper.Wrappers;
using BookingPhongHoc.Dtos;
using BookingPhongHoc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


namespace BookingPhongHoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingsService _bookingsService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(BookingsService bookingsService, ILogger<BookingsController> logger)
        {
            _bookingsService = bookingsService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("get-all-bookings")]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var bookingsData = await _bookingsService.GetAllBookings();
                return Ok(new { bookingsData });
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

        [HttpGet("rooms/{roomId}")]
        public async Task<IActionResult> GetRoomById(string roomId)
        {
            var room = await _bookingsService.GetRoomById(roomId);
            return Ok(room);
        }

        [HttpGet("teachers/{teacherId}")]
        public async Task<IActionResult> GetTeacherById(string teacherId)
        {
            var teacher = await _bookingsService.GetTeacherById(teacherId);
            return Ok(teacher);
        }

        [Authorize]
        [HttpPost("create-booking")]

        public async Task<IActionResult> CreateBooking([FromBody] Bookings booking)
        {
            try
            {
                var createdBooking = await _bookingsService.CreateBooking(booking);
                return Ok(createdBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
