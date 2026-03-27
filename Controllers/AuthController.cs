using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendationApp.Models;
using MovieRecommendationApp.Repositories;
using MovieRecommendationApp.Services;

namespace MovieRecommendationApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _authService.Login(request.Login, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            return Ok(new
            {
                userId = user.Id,
                login = user.Login,
                message = "Вход выполнен успешно"
            });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var success = _authService.Register(request.Login, request.Password, request.Email);

            if (!success)
            {
                return BadRequest(new { message = "Ошибка регистрации. Возможно, логин уже существует или данные некорректны." });
            }

            return Ok(new { message = "Регистрация успешна" });
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}