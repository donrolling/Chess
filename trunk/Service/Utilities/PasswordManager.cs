using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Service.Utilities {
	public static class PasswordManager {
		private static int passwordLength = 8;
		private static int saltLength = 8;

		public static HashedPasswordAndSalt GetHashedPasswordAndSalt() {
			var password = createRandomString(passwordLength);
			return GetHashedPasswordAndSalt(password);
		}
		public static HashedPasswordAndSalt GetHashedPasswordAndSalt(string password) {
			var salt = createRandomString(saltLength);
			return GetHashedPasswordAndSalt(password, salt);
		}
		public static HashedPasswordAndSalt GetHashedPasswordAndSalt(string password, string salt) {
			var hashedPassword = createPasswordHash(password, salt);
			return new HashedPasswordAndSalt { HashedPassword = hashedPassword, Salt = salt };
		}
		public static bool ComparePasswordWithHashedPassword(string password, string salt, string hashedPassword) {
			var hPassword = createPasswordHash(password, salt);
			return hPassword == hashedPassword;
		}
		private static string createRandomString(int size) {
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];
			rng.GetBytes(buff);
			return Convert.ToBase64String(buff);
		}
		private static string createPasswordHash(string password, string salt) {
			HashAlgorithm algorithm = new SHA256Managed();

			var asciiEnc = new ASCIIEncoding();
			var passwordBytes = asciiEnc.GetBytes(password);
			var plainSalt = asciiEnc.GetBytes(salt);

			byte[] plainTextWithSaltBytes = new byte[passwordBytes.Length + plainSalt.Length];

			for (int i = 0; i < passwordBytes.Length; i++) {
				plainTextWithSaltBytes[i] = passwordBytes[i];
			}
			for (int i = 0; i < salt.Length; i++) {
				plainTextWithSaltBytes[passwordBytes.Length + i] = plainSalt[i];
			}

			var hashBytes = algorithm.ComputeHash(plainTextWithSaltBytes);
			return System.Text.Encoding.UTF8.GetString(hashBytes);
		}
	}

	public class HashedPasswordAndSalt {
		public string HashedPassword { get; set; }
		public string Salt { get; set; }	
	}
}