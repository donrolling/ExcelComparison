namespace Business.Models.Membership {
	public class PasswordDTO {
		public string EncryptedPassword { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
		public const int PasswordMaxLength = 16;
		public const int PasswordMinLength = 8;
		public const int SaltLength = 32;
	}
}