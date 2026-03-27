using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Data;
using MovieRecommendationApp.Models;

namespace MovieRecommendationApp.Repositories
{
    public class UserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        // Получить пользователя по логину
        public User GetUserByLogin(string login)
        {
            return _context.Users.FirstOrDefault(u => u.Login == login);
        }

        // Получить пользователя по ID
        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        // Добавить нового пользователя
        public bool AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Проверить, существует ли пользователь
        public bool UserExists(string login)
        {
            return _context.Users.Any(u => u.Login == login);
        }
    }
}