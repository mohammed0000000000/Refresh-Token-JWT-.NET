using refreshTokenJWT.Models;
using System.IdentityModel.Tokens.Jwt;

namespace refreshTokenJWT.Repository
{
	public interface IJwtServices
	{
		Task<JwtSecurityToken> GenerateToken(User user);
		RefreshToken GenerateRefreshToken();
	}
}
