using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            return _context.Rooms.Include(r => r.Hotel.City).Where(r => r.HotelId == HotelId).Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                Name = r.Name,
                Capacity = r.Capacity,
                Image = r.Image,
                Hotel = new HotelDto
                {
                    HotelId = r.HotelId,
                    Name = r.Hotel.Name,
                    Address = r.Hotel.Address,
                    CityId = r.Hotel.CityId,
                    CityName = r.Hotel.City.Name,
                    State = r.Hotel.City.State
                }
            });
        }

        public RoomDto AddRoom(Room room) {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return _context.Rooms.Include(r => r.Hotel.City).Where(r => r.RoomId == room.RoomId).Select(r => new RoomDto {
                RoomId = r.RoomId,
                Name = r.Name,
                Capacity = r.Capacity,
                Image = r.Image,
                Hotel = new HotelDto
                {
                    HotelId = r.HotelId,
                    Name = r.Hotel.Name,
                    Address = r.Hotel.Address,
                    CityId = r.Hotel.CityId,
                    CityName = r.Hotel.City.Name,
                    State = r.Hotel.City.State
                }
            }).FirstOrDefault();
        }

        public void DeleteRoom(int RoomId) {
            var room = _context.Rooms.Find(RoomId);
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
    }
}