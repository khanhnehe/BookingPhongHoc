using System.ComponentModel.DataAnnotations;

namespace BookingPhongHoc.Dtos
{
    public class CreateBooking
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string StatusBooking { get; set; }

        public string RoomId { get; set; }
        public string TeacherId { get; set; }

        //public string? RoomName { get; set; }
        //public int? Capacity { get; set; }
        //public string? TeacherName { get; set; }

        public string[]? IdOfTeacher { get; set; }
    }
    

}
