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

namespace BookingPhongHoc.Services
{
    public class RoomsService : AirtableBaseService
    {
        public RoomsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration, configuration["Airtable:Tables:Rooms"]) { }

        public async Task<RoomsData> GetAllRooms()
        {
            var url = GetUrl();
            var response = await SendAsync(HttpMethod.Get, url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var roomsData = JsonConvert.DeserializeObject<RoomsData>(responseContent);

            if (roomsData != null && roomsData.Records != null)
            {
                roomsData.Records = roomsData.Records
                                    .OrderByDescending(r => r.CreatedTime)
                                    .ToArray();
            }

            return roomsData;
        }


        public async Task<bool> RoomExist(string roomName)
        {
            var roomsData = await GetAllRooms();
            foreach (var room in roomsData.Records)
            {
                if (room.Fields.RoomName == roomName)
                {
                    return true;
                }
            }
            return false;
        }


        public async Task<Rooms> CreateRoom(Rooms input)
        {
            if (await RoomExist(input.RoomName))
            {
                throw new Exception("Tên phòng đã tồn tại");
            }

            var record = new { records = new[] { new { fields = input } } };
            var url = GetUrl();

            var response = await SendJsonAsync(HttpMethod.Post, url, record);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Có lỗi xảy ra khi tạo phòng mới");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdRoom = JsonConvert.DeserializeObject<RoomsData>(responseContent).Records[0].Fields;

            return createdRoom;
        }

        public async Task DeleteRoom(string id)
        {
            var RoomsData = await GetAllRooms();
            var room = RoomsData.Records.FirstOrDefault(t => t.Id == id);

            if (room == null)
            {
                throw new Exception("Không tìm thấy phòngnày");
            }

            var url = GetUrl() + "/" + id;

            var response = await SendAsync(new HttpMethod("DELETE"), url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Có lỗi xảy ra khi xóa phòng");
            }
        }

        public async Task<Rooms> UpdateRoom(string id, Rooms input)
        {
            var RoomsData = await GetAllRooms();
            var room = RoomsData.Records.FirstOrDefault(t => t.Id == id);

            if (room == null)
            {
                throw new Exception("Không tìm thấy phòng này");
            }

            if (room.Fields.RoomName != input.RoomName && await RoomExist(input.RoomName))
            {
                throw new Exception("Tên phòng đã tồn tại");
            }

            var record = new { records = new[] { new { id, fields = input } } };
            var url = GetUrl();

            var response = await SendJsonAsync(new HttpMethod("PATCH"), url, record);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Có lỗi xảy ra khi cập nhật phòng");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedRoom = JsonConvert.DeserializeObject<RoomsData>(responseContent).Records[0].Fields;

            return updatedRoom;
        }


    }
}
