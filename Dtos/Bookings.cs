using System;
using System.ComponentModel.DataAnnotations;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Dtos
{
    public class Bookings
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public int? StatusBooking { get; set; }

        public string RoomId { get; set; }
        public string TeacherId { get; set; }

        public string[]? IdOfTeacher { get; set; }
        public string? NameOfTeacher { get; set; }

        public string[]? IdOfRoom { get; set; }
        public string? NameOfRoom { get; set; }
    }

    public class BookingFields
    {
        public string Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public Bookings Fields { get; set; }
    }

    public class BookingsData
    {
        public BookingFields[] Records { get; set; }
    }
}
