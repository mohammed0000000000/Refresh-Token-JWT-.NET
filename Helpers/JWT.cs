namespace refreshTokenJWT.Helpers
{
	public class JWT
	{
		public string SecrityKey { get; set; }
		public string AudienceIP { get; set; }
		public string IssuerIp { get; set; }
		public int AccessTokenExpired { get; set; }
		public int RefreshTokenExpired { get; set; }
	}
}
