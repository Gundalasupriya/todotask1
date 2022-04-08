using TodoTask.DTOs;
using TodoTask.Models;
using TodoTask.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JwtApp.Models;

namespace TodoTask.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private readonly IConfiguration _configuration;
  public UserController(ILogger<UserController> logger, IUserRepository User, IConfiguration configuration)
    {
        _logger = logger;
        _user = User;
        _configuration = configuration;

    }
    // [HttpGet]
    // public async Task<ActionResult<List<UserDTO>>> GetList()
    // {
    //     var res = await _user.GetList();
    //     return Ok(res.Select(x => x.asDTO));
    // }

    [HttpGet("{user_id}")]

    public async Task<ActionResult<UserDTO>> GetById([FromRoute] long user_id)
    {
        var res = await _user.GetById(user_id);
        if (res == null)
         return NotFound("No Product found with given employee number");
        var dto = res.asDTO;

        return Ok(dto);
    }

    [HttpPost("Create")]

    public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateDTO Data)
    {

        var toCreateUser = new User
        {
            Name = Data.Name.Trim(),

            Password = Data.Password.Trim(),
        };
        var createdUser = await _user.Create(toCreateUser);

        return StatusCode(StatusCodes.Status201Created, createdUser.asDTO);
    }

    [HttpPut("{user_id}")]
    public async Task<ActionResult> UpdateUser([FromRoute] long user_id,
    [FromBody] UserCreateDTO Data)
    {
        var existing = await _user.GetById(user_id);
        if (existing is null)
            return NotFound("No Product found with given customer number");

        var toUpdateUser = existing with
        {

        };

        var didUpdate = await _user.Update(toUpdateUser);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update");
        return NoContent();
    }


    [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] UserLogin userLogin)
        {
            var user = await _user.GetByname(userLogin.name);
            if (user == null)
            // {
               
            // }
            return NotFound("User not found");
            if(user.Password != userLogin.Password)
            return Unauthorized("Invalid Password");
            var token = Generate(user);
                return Ok(token);
        }

        private string Generate(User user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new  Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                 new  Claim(ClaimTypes.Name,user.Name),
                new  Claim(ClaimTypes.Name,user.Name),
                // new  Claim(ClaimTypes.GivenName,user.GivenName),
                // new  Claim(ClaimTypes.Surname,user.Surname),
                // new  Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires:DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        // private async ActionResult(UserLogin userlogin)
        // {
        // //     var currentUser = UserConstants.Users.FirstOrDefault(o => o.Username.ToLower() ==
        // //    userlogin.Username.ToLower() && o.Password == userlogin.Password );
          
        //   var currentUser = await _user.GetByUsername(userlogin.Username);
        //   if(currentUser.Password != userlogin.Password)
        //   {
        //       return currentUser;
        //   })
          
        //    if(currentUser != null)
        //    {
        //        return currentUser;
        //    }
        //    return null;
        // }
    
}   
    



      