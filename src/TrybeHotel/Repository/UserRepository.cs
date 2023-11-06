using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto? GetUserById(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new UserDto {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                UserType = u.UserType
            }).FirstOrDefault();
        }

        public UserDto? Login(LoginDto login)
        {
           var user = GetUserByEmail(login.Email);
           if (user == null) return null;
           var password = _context.Users.Where(u => u.UserId == user.UserId).First().Password;
           if (login.Password != password) return null;
           return user;
        }
        public UserDto? Add(UserDtoInsert user)
        {
            if (GetUserByEmail(user.Email) != null) return null;
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return new UserDto
            {
                UserId = newUser.UserId,
                Name = newUser.Name,
                Email = newUser.Email,
                UserType = newUser.UserType
            };

        }

        public UserDto? GetUserByEmail(string userEmail)
        {
            return _context.Users.Where(u => u.Email == userEmail).Select(u => new UserDto {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                UserType = u.UserType
            }).FirstOrDefault();
        }

        public IEnumerable<UserDto> GetUsers()
        {
           return _context.Users.Select(u => new UserDto {
               UserId = u.UserId,
               Name = u.Name,
               Email = u.Email,
               UserType = u.UserType
           }).ToList();
        }

    }
}