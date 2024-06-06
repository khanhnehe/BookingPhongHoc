namespace BookingPhongHoc.Dtos
{
    public class Rooms
    {
        public class Room
        {
            public string RoomName { get; set; }
            public string Id { get; set; }
            public int Capacity { get; set; }
            public string Description { get; set; }
            public List<string> Booking { get; set; }
        }

    }
}
