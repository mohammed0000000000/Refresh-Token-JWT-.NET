using refreshTokenJWT.DTOs;
using refreshTokenJWT.Models;

namespace refreshTokenJWT.Repository
{
	interface IAuthService
	{
		Task<AuthModel> RegisterAsync(RegisterDto model);
		Task<AuthModel> LoginAsync(LoginDto login);
		Task<AuthModel> RefreshTokenAsync(string token);
		Task<bool> RefreshTokenInvokeAsync(string token);
	}
}
