using Softin_HotelBookingAPI.Data;
using Softin_HotelBookingAPI.Interfaces;
using Softin_HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Softin_HotelBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Booking> _bookingRepository;

        public BookingService(
            IRepository<Room> roomRepository,
            IRepository<Booking> bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<Booking> CreateBookingAsync(BookingRequest bookingRequest)
        {
            var room = await _roomRepository.GetByIdAsync(bookingRequest.RoomId);

            if (room == null)
                throw new ArgumentException("Room not found");

            if (bookingRequest.CheckOutDate <= bookingRequest.CheckInDate)
                throw new ArgumentException("Check-out date must be after check-in date");

            // Check for overlapping bookings
            var existingBookings = await _bookingRepository.GetAllAsync();

            if (existingBookings.Any(b => b.RoomId == bookingRequest.RoomId &&
                ((bookingRequest.CheckInDate >= b.CheckInDate && bookingRequest.CheckInDate < b.CheckOutDate) ||
                 (bookingRequest.CheckOutDate > b.CheckInDate && bookingRequest.CheckOutDate <= b.CheckOutDate) ||
                 (bookingRequest.CheckInDate <= b.CheckInDate && bookingRequest.CheckOutDate >= b.CheckOutDate))))
            {
                throw new InvalidOperationException("Room is already booked for the selected dates, please choose another dates or room. Thank you!");
            }

            var booking = new Booking
            {
                GuestName = bookingRequest.GuestName,
                RoomId = bookingRequest.RoomId,
                CheckInDate = bookingRequest.CheckInDate,
                CheckOutDate = bookingRequest.CheckOutDate
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return booking;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsByDateAsync(DateTime checkIn, DateTime checkOut)
        {
            if (checkOut < checkIn)
                throw new ArgumentException("Check-out date must be after check-in date");

            var allRooms = await _roomRepository.GetAllAsync();
            var allBookings = await _bookingRepository.GetAllAsync();

            var availableRooms = allRooms.Where(room =>
                !allBookings.Any(booking =>
                    booking.RoomId == room.Id &&
                    (
                        (checkIn >= booking.CheckInDate && checkIn < booking.CheckOutDate) ||
                        (checkOut > booking.CheckInDate && checkOut <= booking.CheckOutDate) ||
                        (checkIn <= booking.CheckInDate && checkOut >= booking.CheckOutDate)
                    )
                )).ToList();

            return availableRooms;
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            await _bookingRepository.DeleteAsync(booking);

            var room = await _roomRepository.GetByIdAsync(booking.RoomId);
            if (room != null)
            {
                var bookings = await _bookingRepository.GetAllAsync();
                bool hasOtherBookings = bookings.Any(b => b.RoomId == room.Id && b.Id != bookingId);
                if (!hasOtherBookings)
                {
                    room.IsAvailable = true;
                    await _roomRepository.UpdateAsync(room);
                }
            }

            await _bookingRepository.SaveChangesAsync();
            return true;
        }


    }
}
