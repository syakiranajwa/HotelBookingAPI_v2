using Softin_HotelBookingAPI.Models;

namespace Softin_HotelBookingAPI.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Room>> GetAvailableRoomsAsync();
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking> CreateBookingAsync(BookingRequest bookingRequest);
        Task<IEnumerable<Room>> GetAvailableRoomsByDateAsync(DateTime checkIn, DateTime checkOut);
        Task<bool> CancelBookingAsync(int bookingId);

    }
}
