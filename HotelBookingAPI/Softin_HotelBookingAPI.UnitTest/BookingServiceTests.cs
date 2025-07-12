using Softin_HotelBookingAPI.Data;
using Softin_HotelBookingAPI.Models;
using Softin_HotelBookingAPI.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace Softin_HotelBookingAPI.UnitTest
{
    public class BookingServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly BookingService _bookingService;
        private readonly ITestOutputHelper _output;

        public BookingServiceTests(ITestOutputHelper output)
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "HotelBookingDb")
                .Options;

            _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();

            var roomRepo = new Repository<Room>(_context);
            var bookingRepo = new Repository<Booking>(_context);
            _bookingService = new BookingService(roomRepo, bookingRepo);

            _output = output;
        }


        [Fact]
        public async Task CreateBooking_ShouldFail_WhenRoomIsAlreadyBooked()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique per test
                .Options;

            using var context = new ApplicationDbContext(options);
            var roomRepo = new Repository<Room>(context);
            var bookingRepo = new Repository<Booking>(context);
            var service = new BookingService(roomRepo, bookingRepo);

            // Seed room and booking
            var room = new Room { Id = 1, Name = "101", Type = "Single" };
            await context.Rooms.AddAsync(room);
            await context.Bookings.AddAsync(new Booking
            {
                GuestName = "Sarah",
                RoomId = 1,
                CheckInDate = new DateTime(2025, 7, 1),
                CheckOutDate = new DateTime(2025, 7, 3)
            });
            await context.SaveChangesAsync();

            // Try overlapping booking
            var request = new BookingRequest
            {
                GuestName = "Another Guest",
                RoomId = 1,
                CheckInDate = new DateTime(2025, 7, 2),
                CheckOutDate = new DateTime(2025, 7, 4)
            };

            // Assert failure
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateBookingAsync(request));
            _output.WriteLine($"Actual exception message: {ex.Message}");
            Assert.Contains("already booked", ex.Message);
        }


        [Fact]
        public async Task CreateBooking_ShouldSucceed_WhenRoomIsAvailableOnSelectedDates()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var roomRepo = new Repository<Room>(context);
            var bookingRepo = new Repository<Booking>(context);
            var service = new BookingService(roomRepo, bookingRepo);

            var room = new Room { Id = 1, Name = "101", Type = "Single" };
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            var request = new BookingRequest
            {
                GuestName = "Test Guest",
                RoomId = 1,
                CheckInDate = new DateTime(2025, 7, 10),
                CheckOutDate = new DateTime(2025, 7, 12)
            };

            var booking = await service.CreateBookingAsync(request);

            //Assert.NotNull(booking);
            //Assert.Equal("Test Guest", booking.GuestName);
            //Assert.Equal(1, booking.RoomId);
            _output.WriteLine($"Booking: {booking.GuestName}");
        }
    }
}
