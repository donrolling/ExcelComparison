using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services.Membership
{
	public class Rfc2898
	{
		public const int CMinIterations = 1000;

		public const int CMinSaltLength = 8;

		private readonly HMACSHA512 _hmacsha512Obj;

		private readonly int c;
		private readonly int hLen;

		private readonly byte[] P;

		private readonly byte[] S;
		private int dkLen;

		public Rfc2898(byte[] password, byte[] salt, int iterations)
		{
			if (iterations < 1000) {
				throw new Exception("Iteration count is less than the 1000 recommended in Rfc2898");
			}
			if ((int)salt.Length < 8) {
				throw new Exception("Salt is less than the 8 byte size recommended in Rfc2898");
			}
			this._hmacsha512Obj = new HMACSHA512(password);
			this.hLen = this._hmacsha512Obj.HashSize / 8;
			this.P = password;
			this.S = salt;
			this.c = iterations;
		}

		public Rfc2898(string password, byte[] salt, int iterations)
			: this((new UTF8Encoding(false)).GetBytes(password), salt, iterations)
		{
		}

		public Rfc2898(string password, string salt, int iterations)
			: this((new UTF8Encoding(false)).GetBytes(password), (new UTF8Encoding(false)).GetBytes(salt), iterations)
		{
		}

		public static byte[] PBKDF2(byte[] P, byte[] S, int c, int dkLen)
		{
			return (new Rfc2898(P, S, c)).GetDerivedKeyBytes_PBKDF2_HMACSHA512(dkLen);
		}

		public byte[] GetDerivedKeyBytes_PBKDF2_HMACSHA512(int keyLength)
		{
			this.dkLen = keyLength;
			double num = Math.Ceiling((double)this.dkLen / (double)this.hLen);
			byte[] numArray = new byte[0];
			for (int i = 1; (double)i <= num; i++) {
				numArray = this.pMergeByteArrays(numArray, this.F(this.P, this.S, this.c, i));
			}
			return numArray.Take<byte>(this.dkLen).ToArray<byte>();
		}

		private byte[] F(byte[] P, byte[] S, int c, int i)
		{
			byte[] numArray = this.pMergeByteArrays(S, this.INT(i));
			byte[] numArray1 = this.PRF(P, numArray);
			byte[] numArray2 = numArray1;
			for (int num = 1; num < c; num++) {
				numArray1 = this.PRF(P, numArray1);
				for (int j = 0; j < (int)numArray1.Length; j++) {
					numArray2[j] = (byte)(numArray2[j] ^ numArray1[j]);
				}
			}
			return numArray2;
		}

		private byte[] INT(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(bytes);
			}
			return bytes;
		}

		private byte[] pMergeByteArrays(byte[] source1, byte[] source2)
		{
			byte[] numArray = new byte[(int)source1.Length + (int)source2.Length];
			Buffer.BlockCopy(source1, 0, numArray, 0, (int)source1.Length);
			Buffer.BlockCopy(source2, 0, numArray, (int)source1.Length, (int)source2.Length);
			return numArray;
		}

		private byte[] PRF(byte[] P, byte[] S)
		{
			return this._hmacsha512Obj.ComputeHash(this.pMergeByteArrays(P, S));
		}
	}
}