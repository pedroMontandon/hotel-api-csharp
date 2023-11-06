using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse? Add(BookingDtoInsert booking, string email)
        {
            var room = GetRoomById(booking.RoomId);
            if (room.Capacity < booking.GuestQuant) return null;
            var user = _context.Users.Where(u => u.Email == email).First();
            var bookingEntity = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                RoomId = booking.RoomId,
                UserId = user.UserId,
            };
            _context.Bookings.Add(bookingEntity);
            _context.SaveChanges();
            return new BookingResponse
            {
                BookingId = bookingEntity.BookingId,
                CheckIn = bookingEntity.CheckIn,
                CheckOut = bookingEntity.CheckOut,
                GuestQuant = bookingEntity.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City.Name,
                        State = room.Hotel.City.State
                    }
                }
            };
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            var booking = _context.Bookings.Include(b => b.Room).Include(b => b.Room.Hotel).Include(b => b.Room.Hotel.City).Where(b => b.BookingId == bookingId).First();
            var user = _context.Users.Where(u => u.Email == email).First();
            if (booking.UserId != user.UserId) return null;
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = booking.Room.RoomId,
                    Name = booking.Room.Name,
                    Capacity = booking.Room.Capacity,
                    Image = booking.Room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = booking.Room.Hotel.HotelId,
                        Name = booking.Room.Hotel.Name,
                        Address = booking.Room.Hotel.Address,
                        CityId = booking.Room.Hotel.CityId,
                        CityName = booking.Room.Hotel.City.Name,
                        State = booking.Room.Hotel.City.State
                    }
                }
            };
        }

        public Room GetRoomById(int RoomId)
        {
            return _context.Rooms.Include(r => r.Hotel).Include(r => r.Hotel.City).Where(r => RoomId == r.RoomId).First();
        }
    }

}