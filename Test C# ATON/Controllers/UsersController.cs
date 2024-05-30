using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers
{
    [Controller]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //(Admin only)
        //Create a new user
        //WORK
        [HttpPost]
        public IActionResult CreateUser([FromHeader] string login, [FromHeader] string password, [FromBody] User user)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can create users");
            }

            if (_userRepository.GetUserByLogin(user.Login) != null)
            {
                return BadRequest("Login already exists");
            }

            user.CreatedBy = login;
            _userRepository.AddUser(user);
            return Ok(user);
        }

        //Upadate user params
        //WORK
        [HttpPut]
        public IActionResult UpdateUser([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin, [FromBody] User userUpdate)
        {
            var requestor = _userRepository.GetUserByLoginAndPassword(login, password);
            var user = _userRepository.GetUserByLogin(userLogin);

            if (requestor == null || user == null || (!requestor.Admin && requestor.Login != userLogin))
            {
                return Unauthorized();
            }

            if (user.RevokedOn != null)
            {
                return BadRequest("User is deactivated");
            }

            user.Name = userUpdate.Name;
            user.Gender = userUpdate.Gender;
            user.Birthday = userUpdate.Birthday;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = login;
            _userRepository.UpdateUser(user);
            return Ok(user);
        }

        //Change user password
        //WORK
        [HttpPut]
        public IActionResult ChangePassword([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin, [FromBody] string newPassword)
        {
            var requestor = _userRepository.GetUserByLoginAndPassword(login, password);
            var user = _userRepository.GetUserByLogin(userLogin);

            if (requestor == null || user == null || (!requestor.Admin && requestor.Login != userLogin))
            {
                return Unauthorized("Admin's if");
            }

            if (user.RevokedOn != null)
            {
                return BadRequest("User is deactivated.");
            }

            user.Password = newPassword;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = login;
            _userRepository.UpdateUser(user);
            return Ok(user);
        }

        //(Admin only)
        //Get all active users
        //WORK
        [HttpGet]
        public IActionResult GetAllActiveUsers([FromHeader] string login, [FromHeader] string password)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can view all active users");
            }

            var users = _userRepository.GetAllActiveUsers().OrderBy(u => u.CreatedOn);
            return Ok(users);
        }

        //(Admin only)
        //Get user by login
        //WORK
        [HttpGet]
        public IActionResult GetUserByLogin([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can view the user");
            }

            var user = _userRepository.GetUserByLogin(userLogin);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new {user.Name, user.Gender, user.Birthday, IsActive = user.RevokedOn == null});
        }

        //Get user by login abd password
        //WORK
        [HttpGet]
        public IActionResult GetUserDetails([FromHeader] string login, [FromHeader] string password)
        {
            var user = _userRepository.GetUserByLoginAndPassword(login, password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials or user is deactivated");
            }

            return Ok(new { user.Name, user.Gender, user.Birthday, IsActive = user.RevokedOn == null });
        }

        //(Admin only)
        //Get users older than specified age
        //WORK
        [HttpGet]
        public IActionResult GetUsersOlderThan([FromHeader] string login, [FromHeader] string password, [FromHeader] int age)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if(admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can view users older than specified age");
            }

            var users = _userRepository.GetUsersOlderThan(age);
            return Ok(users);
        }

        //(Admin only)
        //Soft delete user
        //WORK
        [HttpDelete]
        public IActionResult SoftDeleteUser([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                Unauthorized("Only admins can delete users");
            }

            _userRepository.SoftDeleteUser(userLogin, login);
            return Ok();
        }

        //(Admin only)
        //Restore user
        //WORK
        [HttpPut]
        public IActionResult RestoreUser([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can restore users");
            }

            _userRepository.RestoreUser(userLogin);
            return Ok();
        }

        //(Admin only)
        //Delete User
        //WORK
        [HttpDelete]
        public IActionResult DeleteUser([FromHeader] string login, [FromHeader] string password, [FromHeader] string userLogin)
        {
            var admin = _userRepository.GetUserByLoginAndPassword(login, password);
            if (admin == null || !admin.Admin)
            {
                return Unauthorized("Only admins can delete users");
            }

            _userRepository.DeleteUser(userLogin);
            return Ok();
        }
    }
}