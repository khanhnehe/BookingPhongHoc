using System.Threading.Tasks;
using BookingPhongHoc.Dtos;

namespace BookingPhongHoc.Repositories
{
    public interface IBookingRepository
    {
        Task<BookingFields[]> GetAllBookingsAsync();
        Task<BookingFields[]> GetPendingBookingsAsync();
    }
}
