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

        //
        public async Task<BookingsData> GetBookingsByRoomAndTime(string roomId, DateTime startTime, DateTime endTime)
        {
            var url = GetUrl().SetQueryParam("filterByFormula", $"AND({roomId} = RoomId, {startTime} >= StartTime, {endTime} <= EndTime)");
            var response = await SendAsync(HttpMethod.Get, url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var bookingsData = JsonConvert.DeserializeObject<BookingsData>(responseContent);
            return bookingsData;
        }

        public async Task<string> GetTeacherName(string teacherId)
        {
            // Tạo URL cho yêu cầu API
            var url = GetUrl(teacherId);

            // Thực hiện yêu cầu API
            var response = await SendAsync(HttpMethod.Get, url);

            // Đọc và phân tích cú pháp dữ liệu trả về
            var responseContent = await response.Content.ReadAsStringAsync();
            var teacherData = JsonConvert.DeserializeObject<TeachersData>(responseContent);

            // Kiểm tra xem dữ liệu giáo viên có tồn tại không
            if (teacherData?.Records[0]?.Fields?.TeacherName == null)
            {
                throw new Exception("Không thể lấy thông tin giáo viên");
            }

            return teacherData.Records[0].Fields.TeacherName;
        }



        public async Task<Bookings> CreateBooking(Bookings input)
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

            //input.TeacherName = teacher.TeacherName;
            // Lấy tên giáo viên từ Airtable API
            var teacherName = await GetTeacherName(input.TeacherId);

            // Kiểm tra xem teacherName có phải là null không
            if (teacherName == null)
            {
                throw new Exception("Không thể lấy tên giáo viên");
            }

            // Gán tên giáo viên cho input.TeacherName
            input.TeacherName = new string[] { teacherName };



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
