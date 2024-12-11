using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using refreshTokenJWT.DTOs;
using refreshTokenJWT.Models;
using refreshTokenJWT.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace refreshTokenJWT.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<User> userManager;
		private readonly IConfiguration configuration;
		private readonly IJwtServices jwtServices;
		public AuthService(UserManager<User> userManager, IConfiguration configuration, IJwtServices jwtServices) {
			this.userManager = userManager;
			this.configuration = configuration;
			this.jwtServices = jwtServices;
		}
		public async Task<AuthModel> RegisterAsync(RegisterDto model) {
			try {
				if (await userManager.FindByEmailAsync(model.EmailAddress) is not null)
					return new AuthModel { Message = "Email already exists." };
				if (await userManager.FindByNameAsync(model.UserName) is not null)
					return new AuthModel { Message = "Username already exists." };

				var user = new User { UserName = model.UserName, Email = model.EmailAddress, PasswordHash = model.Password };
				var result = await userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded) {
					StringBuilder errorMessage = new StringBuilder();
					foreach (var error in result.Errors) {
						errorMessage.Append(error.Description);
						errorMessage.Append(" ");
					}
					return new AuthModel { Message = errorMessage.ToString() };

				}

				var token = await jwtServices.GenerateToken(user);
				return new AuthModel {
					Message = "Registration Succesfful",
					Email = model.EmailAddress,
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					//ExpiredOn = token.ValidTo,
					Username = model.UserName,
					IsAuthenticated = true,
				};
			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		public async Task<AuthModel> LoginAsync(LoginDto model) {
			try {
				var user = await userManager.FindByEmailAsync(model.Email);

				if (user is null)
					return new AuthModel() { Message = "InValid User Credentails" };
				if (!await userManager.CheckPasswordAsync(user, model.Password))
					return new AuthModel() { Message = "InValid User Credentails" };
				var token = await jwtServices.GenerateToken(user);

				var authModel = new AuthModel {
					Message = "Login Succesfful",
					Email = model.Email,
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					//ExpiredOn = token.ValidTo,
					IsAuthenticated = true,
				};
				if (user.RefreshTokens.Any(t => t.isActive)) {
					var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.isActive);
					authModel.Token = activeRefreshToken.Token;
					authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
				} else {
					var RefreshToken = jwtServices.GenerateRefreshToken();
					authModel.RefreshToken = RefreshToken.Token;
					authModel.RefreshTokenExpiration = RefreshToken.ExpiresOn;
					user.RefreshTokens.Add(RefreshToken);
					await userManager.UpdateAsync(user);
				}
				return authModel;

			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}
		public async Task<AuthModel> RefreshTokenAsync(string token) {
			var authModel = new AuthModel();
			var user = userManager.Users.SingleOrDefault(x => x.RefreshTokens.Any(t => t.isActive));
			if (user is null) {
				authModel.IsAuthenticated = false;
				authModel.Message = "invalid token";
				return authModel;
			}
			var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
			if (refreshToken.isExpired) {
				authModel.IsAuthenticated = false;
				authModel.Message = "Expired token";
				return authModel;
			}
			// revoked because refresh token use only once
			refreshToken.RevokedOn = DateTime.UtcNow;

			var newRefreshToken = jwtServices.GenerateRefreshToken();
			user.RefreshTokens.Add(newRefreshToken);
			await userManager.UpdateAsync(user);

			var jwtToken = await jwtServices.GenerateToken(user);

			authModel.IsAuthenticated = true;
			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			authModel.Email = user.Email;

			var roles = await userManager.GetRolesAsync(user);
			authModel.Roles = roles.ToList();
			authModel.Username = user.UserName;
			authModel.RefreshToken = newRefreshToken.Token;
			authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

			return authModel;
		}
		public async Task<bool> RefreshTokenInvokeAsync(string token) {
			var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(x => x.Token == token));
			if (user is null)
				return false;
			var refreshToken = user.RefreshTokens.SingleOrDefault(rf => rf.Token == token);
			if (!refreshToken.isActive)
				return false;
			refreshToken.RevokedOn = DateTime.UtcNow;
			await userManager.UpdateAsync(user);
			return true;
		}
	}
}
