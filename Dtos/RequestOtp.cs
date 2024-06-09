using System;
using System.ComponentModel.DataAnnotations;

namespace BookingPhongHoc.Dtos
{
    public class RequestOtp
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public string Code { get; set; }

        public DateTime? SentAt { get; set; }
    }
}
