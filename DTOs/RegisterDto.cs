using System.ComponentModel.DataAnnotations;

namespace refreshTokenJWT.DTOs
{
	public class RegisterDto
	{
		[Required(ErrorMessage = "Your name must be at least 3 characters.")]
		[MinLength(3, ErrorMessage = "You Name Must Be At Least 3 chars")]
		public string UserName { get; set; } = string.Empty;

		[Required]
		[EmailAddress(ErrorMessage = "You Must Provide Valid Eamil")]
		public string EmailAddress { get; set; } = string.Empty;

		[Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#])[A-Za-z\\d@$!%*?&#]{8,}$", ErrorMessage = "You Must Provide Strong Password")]
		//aA1@abcd
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "You Must Confirm Your Password")]
		[Compare(nameof(Password), ErrorMessage = "Not Identitcal To Original Password")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
