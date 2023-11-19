using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreApplication.Interfaces;
using CoreApplication.Model;
using CoreApplication.Infrastructure;
using CoreApplication.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace CoreApplication.Controllers
{

    public class OnboardUserRequest
    {
        public string Username { get; set; }
    }

    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> Authenticate([FromBody] User userParam)
        {
            var user = await _userRepository
                .Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User userParam)
        {
            try
            {
                // save 
                var user = await _userRepository.Create(userParam, userParam.Password);
                return Ok(user);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [NoCache]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {

            //string token = Request.Headers["Authorization"];

            //if (token.StartsWith("Bearer"))
            //{
            //    token = token.Substring("Bearer ".Length).Trim();
            //}
            //var handler = new JwtSecurityTokenHandler();

            //JwtSecurityToken jwt = handler.ReadJwtToken(token);

            //return Unauthorized(jwt);
            
           
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("unverified")]
        public async  Task<ActionResult<IEnumerable<User>>> GetUnverifiedUsers()
        {
            var users = await _userRepository.GetUnverifiedUsers();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPut("onboard")]
        public async Task<ActionResult> OnboardUser([FromBody] OnboardUserRequest request)
        {
            try
            {
                var success = await _userRepository.OnboardUser(request.Username);
                if (success)
                {
                    return Ok(new { message = "User verified successfully." });
                }
                else
                {
                    return NotFound(new { message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        } 
    }
}
