using Microsoft.AspNetCore.Mvc;
using NorthwindSample.Models;
using NorthwindSample.Repositories;

namespace NorthwindSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UsersController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public IActionResult Users(User user)
        {
            _userRepository.Insert(user);
            return Ok(user);
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            var user = _userRepository.GetById(id);
            return Ok(user);
        }
    }
}