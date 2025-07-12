namespace Softin_HotelBookingAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}