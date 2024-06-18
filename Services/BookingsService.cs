using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BookingPhongHoc.Dtos;
using BookingPhongHoc.Repositories;
using static BookingPhongHoc.Enums;
using System.Linq;

namespace BookingPhongHoc.Services
{
    public class BookingsService : AirtableBaseService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<BookingsService> _logger;
        private readonly IConfiguration _config;

        // Constructor khởi tạo BookingsService với các tham số cần thiết
        public BookingsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBookingRepository bookingRepository, ILogger<BookingsService> logger)
           : base(httpClientFactory, configuration, configuration["Airtable:Tables:Bookings"])
        {
            _bookingRepository = bookingRepository;
            _logger = logger;
            _config = configuration;
        }

        // Phương thức lấy tất cả các lịch đặt phòng
        public async Task<BookingFields[]> GetAllBookings()
        {
            return await _bookingRepository.GetAllBookingsAsync();
        }

        // Phương thức lấy thông tin giáo viên theo ID
        public async Task<TeachersFields?> GetTeacherById(string teacherId)
        {
            try
            {
                var url = $"{BaseUrl}/{_config["Airtable:BaseId"]}/{_config["Airtable:Tables:Teachers"]}";
                _logger.LogInformation($"Fetching teachers data from URL: {url}");

                var response = await SendAsync(HttpMethod.Get, url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Không thể lấy dữ liệu giáo viên từ Airtable");
                }

                var content = await response.Content.ReadAsStringAsync();

                var teachersData = JsonConvert.DeserializeObject<TeachersData>(content);

                var teacher = teachersData?.Records?.FirstOrDefault(t => t.Id == teacherId);

                if (teacher != null)
                {
                    return teacher;
                }

                _logger.LogWarning($"Không tìm thấy giáo viên với ID: {teacherId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Đã xảy ra lỗi khi lấy thông tin giáo viên: {ex.Message}");
                return null;
            }
        }

        // Phương thức lấy vai trò của giáo viên theo ID
        public async Task<int> GetTeacherRoleById(string teacherId)
        {
            var teacher = await GetTeacherById(teacherId);
            if (teacher != null && Enum.TryParse<Role>(teacher.Fields.Role.ToString(), out var roleEnum))
            {
                return (int)roleEnum;
            }
            return -1;
        }

        // Phương thức tạo lịch đặt phòng
        public async Task<Bookings> CreateBooking(CreateBooking input)
        {
            if (input.EndTime <= input.StartTime)
            {
                throw new Exception("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");
            }

            var allBookings = await GetAllBookings();

            // Kiểm tra xem có lịch đặt phòng nào cho phòng này trong khoảng thời gian yêu cầu không
            var existingRoomBooking = allBookings.FirstOrDefault(b => b.Fields.RoomId == input.RoomId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingRoomBooking != null)
            {
                if (existingRoomBooking.Fields.StatusBooking == (int)StatusBooking.approved)
                {
                    throw new Exception($"Phòng đã được đặt từ {existingRoomBooking.Fields.StartTime} đến {existingRoomBooking.Fields.EndTime}");
                }
            }

            var existingTeacherBooking = allBookings.FirstOrDefault(b => b.Fields.TeacherId == input.TeacherId
                && ((input.StartTime >= b.Fields.StartTime && input.StartTime <= b.Fields.EndTime)
                    || (input.EndTime >= b.Fields.StartTime && input.EndTime <= b.Fields.EndTime)));

            if (existingTeacherBooking != null)
            {
                if (existingTeacherBooking.Fields.StatusBooking == (int)StatusBooking.approved)
                {
                    throw new Exception($"Giáo viên đã đặt phòng khác từ {existingTeacherBooking.Fields.StartTime} đến {existingTeacherBooking.Fields.EndTime}");
                }
            }

            input.IdOfTeacher = new string[] { input.TeacherId };
            input.IdOfRoom = new string[] { input.RoomId };

            var teacherRole = await GetTeacherRoleById(input.TeacherId);

            switch (teacherRole)
            {
                case 1:
                    input.StatusBooking = StatusBooking.pending;
                    break;
                case 2:
                case 3:
                    input.StatusBooking = StatusBooking.approved;
                    break;
            }

            var record = new { records = new[] { new { fields = input } } };
            var url = GetUrl();

            var response = await SendJsonAsync(HttpMethod.Post, url, record);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Có lỗi xảy ra khi tạo lịch đặt phòng");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdBooking = JsonConvert.DeserializeObject<BookingsData>(responseContent).Records[0];

            return createdBooking.Fields;
        }

        // Phương thức lấy danh sách đặt phòng với trạng thái "pending"
        public async Task<BookingFields[]> GetStatusPending()
        {
            var allBookings = await GetAllBookings();
            return allBookings.Where(b => b.Fields.StatusBooking == (int)StatusBooking.pending).ToArray();
        }
    }
}
