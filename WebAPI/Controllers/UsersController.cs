using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{

	private readonly IUser user;
	public UsersController(IUser user)
	{
		this.user = user;
	}
	[HttpPost("register")]
	public async Task<ActionResult<RegistrationResponse>> RegisterUser(RegisterUserDto registerUserDto)
	{
		var result = await user.RegisterUserAsync(registerUserDto);
		return Ok(result);
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDto loginDto)
	{
		var result = await user.LoginUserAsync(loginDto);
		return Ok(result);
	}
}
