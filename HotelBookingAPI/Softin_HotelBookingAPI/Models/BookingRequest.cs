namespace Softin_HotelBookingAPI.Models
{
    public class BookingRequest
    {
        public string GuestName { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
