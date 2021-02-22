using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble
{
    public class DESCryptoService
    {
		private static readonly string DESIV = "CryDesIv";
		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="originalValue"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string DESEncrypt(string originalValue, string key)
		{
			try
			{
				key += "CryDeKey";
				key = key.Substring(0, 8);
				SymmetricAlgorithm symmetricAlgorithm = new DESCryptoServiceProvider();
				symmetricAlgorithm.Key = Encoding.UTF8.GetBytes(key);
				symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(DESIV);
				ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor();
				byte[] bytes = Encoding.UTF8.GetBytes(originalValue);
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
				return Convert.ToBase64String(memoryStream.ToArray());
			}
			catch
			{
				return originalValue;
			}
		}
		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="encryptedValue"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string DESDecrypt(string encryptedValue, string key)
		{
			try
			{
				key += "CryDeKey";
				key = key.Substring(0, 8);
				SymmetricAlgorithm symmetricAlgorithm = new DESCryptoServiceProvider();
				symmetricAlgorithm.Key = Encoding.UTF8.GetBytes(key);
				symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(DESIV);
				ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor();
				byte[] array = Convert.FromBase64String(encryptedValue);
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			catch
			{
				return encryptedValue;
			}
		}
	}
}
