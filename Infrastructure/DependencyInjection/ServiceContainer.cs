using Application.Contracts;
using Infrastructure.Data;
using Infrastructure.Reponsitories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DependencyInjection;
public static class ServiceContainer
{
	public static IServiceCollection InfrastructureService(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(
			option => option.UseSqlServer(configuration["ConnectionStrings:Default"],
			b => b.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName)),
			ServiceLifetime.Scoped);
		services.AddAuthentication(option => {
			option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(option =>
		{
			option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				ValidIssuer = configuration["Jwt:Issuer"],
				ValidAudience = configuration["Jwt:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
			};
		});
		services.AddScoped<IUser, UserReponsitory>();
		return services;
	}

}
