using AuthAPP.Models;
using AuthAPP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register/userRegistration")]
        public IActionResult RegisterUser(User model)
        {
            try
        {
            if (ModelState.IsValid)
            {
                    model.Role = "User";
                var registeredUser = _authService.RegisterUser(model);
                return Ok(new { Message = "User registered successfully", User = registeredUser });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { Message = "An error occurred while registering the user", Error = ex.Message });
        }
    }
        

        [HttpPost("register/adminRegistration")]
        public IActionResult RegisterAdmin(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Role = "Admin";
                    var registeredUser = _authService.RegisterUser(model);
                    return Ok(new { Message = "User registered successfully", User = registeredUser });
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while registering the user", Error = ex.Message });
            }
        }

        [HttpPost("login/userLogin")]
        public IActionResult LoginUser(UserLogin model)
        {
            try
            {
                // Validate the login model (e.g., check for required fields, validate input)
                if (ModelState.IsValid)
                {
                    // Call the AuthService to perform user login and generate JWT token
                    var user = _authService.LoginUser(model);

                    if (user != null)
                    {
                        var user1 = new User { Username = model.Username, Role = "User" };
                        var token = _authService.GenerateJwtToken(user1);
                        return Ok(new { Token = token });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Invalid username or password" });
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the login request", Error = ex.Message });
            }
        }

        [HttpPost("login/adminLogin")]
        public IActionResult LoginAdmin(UserLogin model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _authService.LoginUser(model);

                    if (user != null)
                    {
                        var user1 = new User { Username = model.Username, Role = "Admin" };
                        var token = _authService.GenerateJwtToken(user1);
                        return Ok(new { Token = token });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Invalid username or password" });
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the login request", Error = ex.Message });
            }
        }
    }
}

