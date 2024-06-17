using System.Threading.Tasks;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Repositories
{
    public interface ITeachersRepository
    {
        Task<Role?> GetTeacherRoleById(string teacherId);
    }
}
