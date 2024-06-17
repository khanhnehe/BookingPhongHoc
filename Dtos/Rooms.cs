using System.ComponentModel.DataAnnotations;

namespace BookingPhongHoc.Dtos
{
    public class Rooms
    {
        [Required]
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;

        public string Description { get; set; }
    }
        public class RoomFields
        {
            public string Id { get; set; }
            public DateTime CreatedTime { get; set; }

            public Rooms Fields { get; set; }
        }

        public class RoomsData
        {
            public RoomFields[] Records { get; set; }
        }
    }

