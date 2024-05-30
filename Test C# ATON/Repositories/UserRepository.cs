using System;
using System.Collections.Generic;
using System.Linq;
using UserService.Models;

namespace UserService.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllActiveUsers();
        User GetUserByLogin(string login);
        User GetUserByLoginAndPassword(string login, string password);
        IEnumerable<User> GetUsersOlderThan(int age);
        void AddUser(User user);
        void UpdateUser(User user);
        void SoftDeleteUser(string login, string revokedBy); // soft delete
        void RestoreUser(string login);
        void DeleteUser(string login); // full delete
    }

    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;
            
        public UserRepository()
        {
            _users = new List<User>() { new User() { Login = "admin", Password = "admin123", Name = "Admin User", Gender = 1, Admin = true, CreatedBy = "system" } };
        }

        public IEnumerable<User> GetAllActiveUsers()
        {
            return _users.Where(u => u.RevokedOn == null);
        }

        public User GetUserByLogin(string login)
        {
            return _users.FirstOrDefault(u => u.Login == login);
        }

        public User GetUserByLoginAndPassword(string login, string password)
        {
            return _users.FirstOrDefault(u => u.Login == login && u.Password == password && u.RevokedOn == null);
        }

        public IEnumerable<User> GetUsersOlderThan(int age)
        {
            var currentDate = DateTime.UtcNow;
            return _users.Where(u => u.Birthday != null && (currentDate.Year - u.Birthday.Value.Year) > age && u.RevokedOn == null);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void UpdateUser(User user)
        {
            var existingUser = GetUserByLogin(user.Login);
            if (existingUser != null)
            {
                existingUser.Login = user.Login;
                existingUser.Gender = user.Gender;
                existingUser.Birthday = user.Birthday;
                existingUser.ModifiedOn = user.ModifiedOn;
                existingUser.ModifiedBy = user.ModifiedBy;
            }
        }

        public void SoftDeleteUser(string login, string revokedBy)
        {
            var user = GetUserByLogin(login);
            if (user != null)
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
            }
        }

        public void RestoreUser(string login)
        {
            var user = GetUserByLogin(login);
            if (user != null )
            {
                user.RevokedOn = null;
                user.RevokedBy = null;
            }
        }

        public void DeleteUser(string login)
        {
            var user = GetUserByLogin(login);
            if (user != null)
            {
                _users.Remove(user);
            }
        }
    }
}
