using System;
using System.ComponentModel.DataAnnotations;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Dtos
{
    public class Teachers
    {
        [Required]
        public string TeacherName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public Role? Role { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Avatar { get; set; }
    }

    public class TeachersFields
    {
        public string Id { get; set; }
        public Teachers Fields { get; set; }
    }

    public class TeachersData
    {
        public TeachersFields[] Records { get; set; }
    }
}
