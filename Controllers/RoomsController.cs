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
    public class RoomsController : ControllerBase
    {
        private readonly RoomsService _roomsService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(RoomsService roomsService, ILogger<RoomsController> logger)
        {
            _roomsService = roomsService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("get-all-rooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var roomsData = await _roomsService.GetAllRooms();
                return Ok(new { roomsData });
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
        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] Rooms room)
        {
            try
            {
                var createdRoom = await _roomsService.CreateRoom(room);
                return Ok(new { createdRoom });
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
        [HttpDelete("delete-room/{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            try
            {
                await _roomsService.DeleteRoom(id);
                return Ok(new { message = "Room deleted successfully" });
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
        [HttpPut("update-room/{id}")]
        public async Task<IActionResult> UpdateRoom(string id, [FromBody] Rooms room)
        {
            try
            {
                var updatedRoom = await _roomsService.UpdateRoom(id, room);
                return Ok(new { updatedRoom });
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
