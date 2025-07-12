using Microsoft.AspNetCore.Mvc;
using Softin_HotelBookingAPI.Interfaces;
using Softin_HotelBookingAPI.Models;

namespace Softin_HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public RoomsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = await _bookingService.GetAvailableRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRoomsByDate([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            try
            {
                var rooms = await _bookingService.GetAvailableRoomsByDateAsync(checkIn, checkOut);
                return Ok(rooms);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }


}
