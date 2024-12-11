using Microsoft.AspNetCore.Identity;

namespace refreshTokenJWT.Models
{
	public class User : IdentityUser
	{
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	}
}
