using CoreAuth.Data;
using CoreAuth.Dtos;
using CoreAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace CoreAuth.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [AllowAnonymous]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _authRepo;
    private readonly IConfiguration _config;

    public AuthController(IAuthRepository authRepo, IConfiguration config)
    {
      _authRepo = authRepo;
      _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
      var user = new User { Name = userDto.Name.ToLower() };

      if (await _authRepo.UserExists(user.Name))
        return BadRequest("Username already exists");

      var userReturn = await _authRepo.Register(user, userDto.Password);

      return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
      var user = await _authRepo.Login(userDto.Name, userDto.Password);

      if (user == null) return Unauthorized();

      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(_config.GetSection("TokenOptions:Key").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Ok(new
      {
        token = tokenHandler.WriteToken(token)
      });
    }

  }
}