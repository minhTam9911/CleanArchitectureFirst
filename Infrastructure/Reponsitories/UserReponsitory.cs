using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reponsitories;
public class UserReponsitory : IUser
{
	private readonly AppDbContext appDbContext;
	private readonly IConfiguration configuration;
	public UserReponsitory(AppDbContext appDbContext, IConfiguration configuration)
	{
		this.appDbContext = appDbContext;
		this.configuration = configuration;
	}

	public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
	{
		var getUser = await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);
		if (getUser == null) return new LoginResponse(false, "User not found");
		bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
		if (checkPassword)
		{
			string jwtToken = GenerateJwtToken(getUser);
				return new LoginResponse(true, "Login successfull",jwtToken);
		}
		else
		{
			return new LoginResponse(false, "Invalid credentials");
		}
	}

	
		
	
	private string GenerateJwtToken(ApplicationUser applicationUser)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		var userClaims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
			new Claim(ClaimTypes.Name, applicationUser.Name),
			new Claim(ClaimTypes.Email, applicationUser.Email)
		};
		var token = new JwtSecurityToken(

			issuer: configuration["Jwt:Issuer"],
			audience: configuration["Jwt:Audience"],
			claims: userClaims,
			expires:DateTime.Now.AddDays(5),
			signingCredentials: credentials
			);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
	{
		var getUser = await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == registerUserDto.Email);
		if (getUser != null)return new RegistrationResponse(false, "User already exist");
		appDbContext.Users.Add(new ApplicationUser() {
			Name = registerUserDto.Name,
			Email = registerUserDto.Email,
			Password =BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password) 
		});
		await appDbContext.SaveChangesAsync();
		return new RegistrationResponse(false,"Register complated");
	}
}
