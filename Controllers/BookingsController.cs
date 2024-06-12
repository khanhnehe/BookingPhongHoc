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
        

        [Authorize]
        [HttpPost("create-booking")]

        public async Task<IActionResult> CreateBooking([FromBody] CreateBooking booking)
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
