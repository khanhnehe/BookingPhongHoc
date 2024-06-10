using System.ComponentModel.DataAnnotations;

namespace BookingPhongHoc.Dtos
{
    public class SignIn
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
