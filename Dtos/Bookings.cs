namespace BookingPhongHoc.Dtos
{
    public class Bookings
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string StatusBooking { get; set; }

        public string RoomId { get; set; }
        public string TeacherId { get; set; }

        public string? RoomName { get; set; }
        public int? Capacity { get; set; }
        public string? TeacherName { get; set; }
    }

    public class BookingFields
    {
        public string Id { get; set; }
        public Bookings Fields { get; set; }
    }

    public class BookingsData
    {
        public BookingFields[] Records { get; set; }
    }
}
