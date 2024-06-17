using System;
using System.Linq;
using System.Threading.Tasks;
using BookingPhongHoc.Dtos;
using Newtonsoft.Json;
using static BookingPhongHoc.Enums;

namespace BookingPhongHoc.Repositories
{
    public class Repository : ITeachersRepository
    {
        private readonly AirtableBaseService _airtableBaseService;

        public Repository(AirtableBaseService airtableBaseService)
        {
            _airtableBaseService = airtableBaseService;
        }

        public async Task<Role?> GetTeacherRoleById(string teacherId)
        {
            try
            {
                var url = _airtableBaseService.GetUrl(teacherId);
                var response = await _airtableBaseService.SendAsync(HttpMethod.Get, url);

                var content = await response.Content.ReadAsStringAsync();
                var teacherData = JsonConvert.DeserializeObject<TeachersData>(content);

                var roleValue = teacherData?.Records?.FirstOrDefault()?.Fields?.Role;

                if (roleValue.HasValue)
                {
                    if (Enum.IsDefined(typeof(Role), roleValue.Value))
                    {
                        return (Role)roleValue.Value;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
