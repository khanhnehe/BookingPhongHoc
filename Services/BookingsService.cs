using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BookingPhongHoc.Dtos;
using BookingPhongHoc.Repositories;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Services
{
    public class BookingsService : AirtableBaseService
    {
        private readonly Repository _repositories;
        private readonly ILogger<BookingsService> _logger;
        private readonly IConfiguration _config;

        // Constructor khởi tạo BookingsService với các tham số cần thiết
        public BookingsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, Repository repositories, ILogger<BookingsService> logger)
           : base(httpClientFactory, configuration, configuration["Airtable:Tables:Bookings"])
        {
            _repositories = repositories;
            _logger = logger;
            _config = configuration;

        }

        // Phương thức lấy tất cả các lịch đặt phòng
        public async Task<BookingsData> GetAllBookings()
        {
            try
            {
                var url = GetUrl();
                _logger.LogInformation($"Fetching bookings from URL: {url}");
                var response = await SendAsync(HttpMethod.Get, url);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response received: {responseContent}");
                var bookingsData = JsonConvert.DeserializeObject<BookingsData>(responseContent);
                if (bookingsData != null && bookingsData.Records != null)
                {
                    bookingsData.Records = bookingsData.Records
                                        .OrderByDescending(r => r.CreatedTime)
                                        .ToArray();
                }
                return bookingsData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in GetAllBookings: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTeacherRoleById(string teacherId)
        {
            try
            {
                var url = $"{BaseUrl}/{_config["Airtable:BaseId"]}/{_config["Airtable:Tables:Teachers"]}";
                _logger.LogInformation($"Fetching teachers data from URL: {url}");

                var response = await SendAsync(HttpMethod.Get, url);

                if (!response.IsSuccessStatusCode)
                {
                    return -1; 
                }

                var content = await response.Content.ReadAsStringAsync();

                var teachersData = JsonConvert.DeserializeObject<TeachersData>(content);

                var teacher = teachersData?.Records?.FirstOrDefault(t => t.Id == teacherId);

                if (teacher != null && Enum.TryParse<Role>(teacher.Fields.Role.ToString(), out var roleEnum))
                {
                    _logger.LogInformation($"Found teacher: {JsonConvert.SerializeObject(teacher)}");
                    return (int)roleEnum; 
                }

                _logger.LogWarning($"No teacher found with ID: {teacherId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching teacher information: {ex.Message}");
            }
            return -1; 
        }




        public async Task<Bookings> CreateBooking(CreateBooking input)
        {
            if (input.EndTime <= input.StartTime)
            {
                throw new Exception("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");
            }

            var allBookings = await GetAllBookings();

            // Kiểm tra xem có lịch đặt phòng nào cho phòng này trong khoảng thời gian yêu cầu không
            var existingRoomBooking = allBookings.Records.FirstOrDefault(b => b.Fields.RoomId == input.RoomId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingRoomBooking != null)
            {
                // Thêm kiểm tra trạng thái đặt phòng là approved
                if (existingRoomBooking.Fields.StatusBooking == 2)
                {
                    throw new Exception($"Bạn không thể đặt lịch vì phòng này đã được đặt trước từ {existingRoomBooking.Fields.StartTime} đến {existingRoomBooking.Fields.EndTime}");
                }
            }

            // Tương tự, kiểm tra xem giáo viên này đã đặt phòng nào khác trong khoảng thời gian yêu cầu không
            var existingTeacherBooking = allBookings.Records.FirstOrDefault(b => b.Fields.TeacherId == input.TeacherId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingTeacherBooking != null)
            {
                // Thêm kiểm tra trạng thái đặt phòng là approved
                if (existingTeacherBooking.Fields.StatusBooking == 2)
                {
                    throw new Exception($"Giáo viên đã đặt phòng khác từ {existingTeacherBooking.Fields.StartTime} đến {existingTeacherBooking.Fields.EndTime} với trạng thái đã được phê duyệt");
                }
            }

            input.IdOfTeacher = new string[] { input.TeacherId };
            input.IdOfRoom = new string[] { input.RoomId };

            var teacherRole = await GetTeacherRoleById(input.TeacherId);

            // Chuyển đổi teacherRole từ chuỗi sang số
            switch (teacherRole)
            {
                case 1:
                    input.StatusBooking = Enums.StatusBooking.pending;
                    break;
                case 2:
                case 3:
                    input.StatusBooking = Enums.StatusBooking.approved;
                    break;
            }

            // Tạo một record mới để gửi đi
            var record = new { records = new[] { new { fields = input } } };
            // Lấy URL để gửi request
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
