using System;
using System.ComponentModel.DataAnnotations;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Dtos
{
    public class Teachers
    {
        //public string Id { get; set; } // Để Id là string

        [Required]
        public string TeacherName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string? Role { get; set; }
        public string? Avatar { get; set; }
    }

    public class TeachersFields
    {
        public Teachers Fields { get; set; }
    }

    public class TeachersData
    {
        public TeachersFields[] Records { get; set; }
    }
}
