namespace BookingPhongHoc.Dtos
{
    public class Teachers
    {
        public string TeacherName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Đổi kiểu dữ liệu Role thành string
    }

    public class TeachersFields
    {
        public Teachers Fields { get; set; }
    }

    public class TeachersData
    {
        public TeachersFields[] Records { get; set; }
    }
}
