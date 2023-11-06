using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {

        private readonly IUserRepository _repository;
        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login){
           var logged = _repository.Login(login);
           if (logged == null) return Unauthorized(new ErrorDto{ Message = "Incorrect e-mail or password"});
           var token = new TokenGenerator().Generate(logged);
           return Ok(new { token });
        }
    }
}