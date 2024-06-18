using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BookingPhongHoc.Dtos;
using Newtonsoft.Json;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AirtableBaseService _airtableBaseService;

        public BookingRepository(AirtableBaseService airtableBaseService)
        {
            _airtableBaseService = airtableBaseService;
        }

        public async Task<BookingFields[]> GetAllBookingsAsync()
        {
            try
            {
                var url = _airtableBaseService.GetUrl();
                var response = await _airtableBaseService.SendAsync(HttpMethod.Get, url);
                var responseContent = await response.Content.ReadAsStringAsync();
                var bookingsData = JsonConvert.DeserializeObject<BookingsData>(responseContent);
                // Adjusted to return an array of BookingFields directly
                return bookingsData?.Records?.OrderByDescending(r => r.CreatedTime).ToArray() ?? Array.Empty<BookingFields>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetAllBookingsAsync: {ex.Message}");
                return Array.Empty<BookingFields>();
            }
        }

        public async Task<BookingFields[]> GetPendingBookingsAsync()
        {
            var allBookings = await GetAllBookingsAsync();
            // Adjusted to filter BookingFields based on the StatusBooking within the nested Bookings object
            return allBookings.Where(b => b.Fields.StatusBooking == (int)StatusBooking.pending).ToArray();
        }
    }
}
