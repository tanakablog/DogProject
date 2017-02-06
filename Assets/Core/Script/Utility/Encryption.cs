namespace Core.Utility
{
	using System;
	using System.Text;
	using System.IO;
	using System.Security.Cryptography;

	/*=================================================
	 * 暗号化・復号化
	 *=================================================*/
	public class AESEncryption {
		//============================================
		// 暗号化
		//============================================
		public static String AES_encrypt(string userID, string uuid, String Input)
		{
			var aes = new RijndaelManaged();
			aes.KeySize = 256;
			aes.BlockSize = 256;
			aes.Padding = PaddingMode.PKCS7;
			aes.Key = makeKey(uuid);
			aes.IV = makeIV(userID);
			
			var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
			byte[] xBuff = null;
			using (var ms = new MemoryStream())
			{
				using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
				{
					byte[] xXml = Encoding.UTF8.GetBytes(Input);
					cs.Write(xXml, 0, xXml.Length);
				}
				
				xBuff = ms.ToArray();
			}
			
			String Output = Convert.ToBase64String(xBuff);
			return Output;
		}
		//============================================
		// 復号化
		//============================================		
		public static String AES_decrypt(string userID,string uuid, String Input)
		{
			RijndaelManaged aes = new RijndaelManaged();
			aes.KeySize = 256;
			aes.BlockSize = 256;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			aes.Key = makeKey(uuid);
			aes.IV = makeIV(userID);
			
			var decrypt = aes.CreateDecryptor();
			byte[] xBuff = null;
			using (var ms = new MemoryStream())
			{
				using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
				{
					byte[] xXml = Convert.FromBase64String(Input);
					cs.Write(xXml, 0, xXml.Length);
				}
				
				xBuff = ms.ToArray();
			}
			
			String Output = Encoding.UTF8.GetString(xBuff);
			return Output;
		}
		//============================================
		// キー作成
		//============================================		
		private static byte[] makeKey(string uuid) {
			string key = uuid.Replace("-", "");
			byte[] byteArray = Encoding.UTF8.GetBytes(key);
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] md5byte = md5.ComputeHash(byteArray);
			string result = BitConverter.ToString(md5byte).Replace("-", "").ToLower();
			return Encoding.UTF8.GetBytes (result);
		}
		//============================================
		// ベクター作成
		//============================================				
		private static byte[] makeIV(string userID) {
			byte[] byteArray = Encoding.UTF8.GetBytes(userID);
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] md5byte = md5.ComputeHash(byteArray);
			string result = BitConverter.ToString(md5byte).Replace("-", "").ToLower();
			return Encoding.UTF8.GetBytes (result);
		}
	}
}
