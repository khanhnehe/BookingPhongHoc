using System.ComponentModel.DataAnnotations;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Dtos
{
    public class CreateBooking
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public StatusBooking? StatusBooking { get; set; }

        public string RoomId { get; set; }
        public string TeacherId { get; set; }
        public string[]? IdOfTeacher { get; set; }
        public string[]? IdOfRoom { get; set; }

    }


}
