using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BookingPhongHoc;
using System.ComponentModel.DataAnnotations;
using Flurl.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;
using BookingPhongHoc.Dtos;
using Microsoft.AspNetCore.Identity;
using Flurl;

namespace BookingPhongHoc.Services
{
    public class BookingsService : AirtableBaseService
    {
        public BookingsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
           : base(httpClientFactory, configuration, configuration["Airtable:Tables:Bookings"]) { }

        public async Task<BookingsData> GetAllBookings()
        {
            var url = GetUrl();
            var response = await SendAsync(HttpMethod.Get, url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var bookingsData = JsonConvert.DeserializeObject<BookingsData>(responseContent);
            return bookingsData;
        }


        public async Task<Bookings> CreateBooking(CreateBooking input)
        {
            // Lấy tất cả các lịch đặt phòng
            var allBookings = await GetAllBookings();

            // Kiểm tra xem phòng học đã được sử dụng trong cùng một khung thời gian hay không
            var existingRoomBooking = allBookings.Records.FirstOrDefault(b => b.Fields.RoomId == input.RoomId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingRoomBooking != null)
            {
                throw new Exception($"Phòng học đã được sử dụng từ {existingRoomBooking.Fields.StartTime} đến {existingRoomBooking.Fields.EndTime}");
            }

            // Kiểm tra xem giáo viên đã được gán cho một lớp học khác trong cùng một khung thời gian hay không
            var existingTeacherBooking = allBookings.Records.FirstOrDefault(b => b.Fields.TeacherId == input.TeacherId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingTeacherBooking != null)
            {
                throw new Exception($"Giáo viên đã đặt phòng khác từ {existingTeacherBooking.Fields.StartTime} đến {existingTeacherBooking.Fields.EndTime}");
            }

            
            
            // Gán tên giáo viên cho input.TeacherName
            // Gán ID giáo viên cho input.TeacherName
            input.IdOfTeacher = new string[] { input.TeacherId };



            // Nếu không có lỗi, tạo một lịch đặt phòng mới
            var record = new { records = new[] { new { fields = input } } };
            var url = GetUrl();

            var response = await SendJsonAsync(new HttpMethod("POST"), url, record);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Có lỗi xảy ra khi tạo lịch đặt phòng");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdBooking = JsonConvert.DeserializeObject<BookingsData>(responseContent).Records[0];


            return createdBooking.Fields;
        }

    }
}
