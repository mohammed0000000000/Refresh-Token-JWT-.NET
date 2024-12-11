using System.ComponentModel.DataAnnotations;

namespace refreshTokenJWT.DTOs
{
	public class LoginDto
	{
		[Required(ErrorMessage = "You Must Provide Email")]
		[EmailAddress(ErrorMessage = " You Must Provide Valid Email")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "You Must Provide Password")]
		public string Password { get; set; } = string.Empty;
	}
}
