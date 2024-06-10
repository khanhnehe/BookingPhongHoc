using System.ComponentModel.DataAnnotations;

namespace BookingPhongHoc.Dtos
{
    public class ChangePassword
    {
        public string? PhoneNumber { get; set; }
        public string? CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
