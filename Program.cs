
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using refreshTokenJWT.Helpers;
using refreshTokenJWT.Models;
using refreshTokenJWT.Repo.EntityFramework.Data;
using refreshTokenJWT.Repository;
using refreshTokenJWT.Services;
using System;
using System.Text;

namespace refreshTokenJWT
{
	public class Program
	{
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
				builder.Configuration.GetConnectionString("connectionStr")
				));
			builder.Services.AddScoped<DbContext, AppDbContext>();
			builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IJwtServices, JwtServices>();
			builder.Services.AddAuthentication(
		   options => {
			   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		   }
	   ).AddJwtBearer(options => {
		   options.SaveToken = false;
		   options.RequireHttpsMetadata = false;
		   options.TokenValidationParameters = new TokenValidationParameters() {
			   ValidateIssuer = true,
			   ValidIssuer = builder.Configuration["JWT:IssuerIp"],
			   ValidateAudience = true,
			   ValidAudience = builder.Configuration["JWT:AudienceIP"],
			   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecrityKey"])),
			   ValidateLifetime = true,
			   ClockSkew = TimeSpan.Zero
		   };
	   });
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment()) {
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
