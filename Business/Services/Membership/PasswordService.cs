using Business.Models.Membership;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services.Membership
{
	public class PasswordService
	{
		public PasswordDTO Generate_EncryptedPassword_Given_Password_Salt(string password, string salt)
		{
			var passwordDTO = new PasswordDTO
			{
				Password = password,
				Salt = salt
			};
			passwordDTO.EncryptedPassword = setEncytpedPassword(passwordDTO.Password, passwordDTO.Salt);
			return passwordDTO;
		}

		public PasswordDTO Generate_EncryptedPassword_Given_Salt(string salt)
		{
			var passwordDTO = new PasswordDTO
			{
				Password = generatePassword(PasswordDTO.PasswordMaxLength),
				Salt = salt
			};
			passwordDTO.EncryptedPassword = setEncytpedPassword(passwordDTO.Password, passwordDTO.Salt);
			return passwordDTO;
		}

		public PasswordDTO Generate_EncryptedPassword_Salt_Given_Password(string password)
		{
			var passwordDTO = new PasswordDTO
			{
				Password = password,
				Salt = generateSalt(PasswordDTO.SaltLength)
			};
			passwordDTO.EncryptedPassword = setEncytpedPassword(passwordDTO.Password, passwordDTO.Salt);
			return passwordDTO;
		}

		public PasswordDTO Generate_NewPassword_EncryptedPassword_Salt()
		{
			var passwordDTO = new PasswordDTO
			{
				Password = generatePassword(PasswordDTO.PasswordMaxLength),
				Salt = generateSalt(PasswordDTO.SaltLength)
			};
			passwordDTO.EncryptedPassword = setEncytpedPassword(passwordDTO.Password, passwordDTO.Salt);
			return passwordDTO;
		}

		private static string createPasswordHash(string pwd, string salt)
		{
			var saltBytes = (new UTF8Encoding(false)).GetBytes(salt);
			var hashedPwd = passwordToHash(saltBytes, pwd);
			var result = (new UTF8Encoding(false)).GetString(hashedPwd);
			return result;
		}

		private static string createSalt(int size)
		{
			var numArray = new byte[size];
			RandomNumberGenerator.Create().GetBytes(numArray);
			return Convert.ToBase64String(numArray);
		}

		private static byte[] passwordToHash(byte[] salt, string password, int iterationCount = 5000)
		{
			if ((int)salt.Length < 8) {
				throw new Exception(string.Concat("The supplied salt is too short, it must be at least ", 8, " bytes long as defined by CMinSaltLength"));
			}
			var utf8Bytes = (new UTF8Encoding(false)).GetBytes(password);
			if ((int)utf8Bytes.Length > 1024) {
				throw new Exception(string.Concat("The supplied password is longer than allowed, it must be smaller than ", 1024, " bytes long as defined by CMaxPasswordLength"));
			}
			return (new Rfc2898(password, salt, iterationCount)).GetDerivedKeyBytes_PBKDF2_HMACSHA512(64);
		}

		private string generatePassword(int length)
		{
			return Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);
		}

		private string generateSalt(int length)
		{
			return createSalt(length);
		}

		private string setEncytpedPassword(string password, string salt)
		{
			return createPasswordHash(password, salt);
		}
	}
}