namespace BookingPhongHoc.Dtos
{
    public class Teachers
    {
        public string TeacherName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public List<string> Booking { get; set; }
    }

}
