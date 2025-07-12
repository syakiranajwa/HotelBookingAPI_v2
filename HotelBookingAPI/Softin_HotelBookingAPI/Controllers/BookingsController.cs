using Microsoft.AspNetCore.Mvc;
using Softin_HotelBookingAPI.Interfaces;
using Softin_HotelBookingAPI.Models;

namespace Softin_HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(BookingRequest bookingRequest)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(bookingRequest);
                return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (!result)
                return NotFound(new { message = "Booking not found" });

            return Ok(new { message = "Booking cancelled successfully" });
        }
    }
}
