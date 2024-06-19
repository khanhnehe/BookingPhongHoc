using System.Threading.Tasks;
using BookingPhongHoc.Dtos;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Repositories
{
    public interface IBookingRepository
    {
        Task<BookingFields[]> GetAllBookingsAsync();
        Task<BookingFields[]> GetPendingBookingsAsync();
    }
}
