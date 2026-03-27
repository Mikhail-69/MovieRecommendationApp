using MovieRecommendationApp.Models;
using MovieRecommendationApp.Repositories;

namespace MovieRecommendationApp.Services
{
    public class AuthService
    {
        private readonly UserRepo _userRepo;

        public AuthService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        // Вход в систему
        public User Login(string login, string password)
        {
            var user = _userRepo.GetUserByLogin(login);

            if (user == null)
                return null;

            if (user.Password != password)
                return null;

            return user;
        }

        // Регистрация нового пользователя
        public bool Register(string login, string password, string email)
        {
            // Проверка на существование пользователя
            if (_userRepo.UserExists(login))
                return false;

            // Валидация
            if (string.IsNullOrEmpty(login) || login.Length < 3)
                return false;

            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return false;

            var newUser = new User
            {
                Login = login,
                Password = password,
                Email = email
            };

            return _userRepo.AddUser(newUser);
        }
    }
}