using System.Text.Json.Serialization;

namespace refreshTokenJWT.Models
{
	public class AuthModel
	{
		public string Message { get; set; } = string.Empty;
		public bool IsAuthenticated { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public List<string> Roles { get; set; } = new List<string>();
		public string Token { get; set; } = string.Empty;
		//public DateTime ExpiredOn { get; set; }
		[JsonIgnore] // ==> ingore this property to send in response
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
	}
}
