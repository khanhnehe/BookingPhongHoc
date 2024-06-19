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

        //
        public async Task<bool> UpdateBooking(string bookingId, object fieldsToUpdate)
        {
            try
            {
                if (string.IsNullOrEmpty(bookingId))
                {
                    return false;
                }

                // Tạo URL để gửi yêu cầu PATCH, bao gồm ID của booking cần cập nhật
                var url = $"{_airtableBaseService.GetUrl()}/{bookingId}";
                // Tạo đối tượng chứa dữ liệu cần cập nhật
                var updateObject = new
                {
                    fields = fieldsToUpdate
                };

                // Chuyển đổi đối tượng cập nhật thành chuỗi JSON
                var jsonContent = JsonConvert.SerializeObject(updateObject);
                // Gửi yêu cầu PATCH đến Airtable API với dữ liệu JSON
                var response = await _airtableBaseService.SendJsonAsync(HttpMethod.Patch, url, jsonContent);

                // Kiểm tra trạng thái phản hồi, nếu không thành công, in ra mã trạng thái và trả về false
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to update booking. Status code: {response.StatusCode}");
                    return false;
                }

                // Nếu cập nhật thành công, in ra thông báo và trả về true
                Console.WriteLine($"Booking updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}"); 
                return false;
            }
        }

    }
}
