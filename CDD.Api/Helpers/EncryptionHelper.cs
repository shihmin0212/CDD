using System.Security.Cryptography;
using System.Text;

namespace CDD.Api.Helpers
{
    public class EncryptionHelper
    {
        /// <summary>
        /// 產生隨機key
        /// </summary>
        /// <param name="keyLength">隨機密鑰的長度（16、24、32位元對應 SHA-128, SHA-192, SHA-256）</param>
        /// <returns>隨機生成的密鑰</returns>
        public static string GenKey(int keyLength, string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            if (keyLength != 16 && keyLength != 24 && keyLength != 32)
                throw new ArgumentException("Invalid key length. Use 16, 24, or 32.");

            var charArray = charSet.Distinct().ToArray();
            char[] result = new char[keyLength];
            for (int i = 0; i < keyLength; i++)
                result[i] = charArray[RandomNumberGenerator.GetInt32(charArray.Length)];
            return new string(result);
        }

        /// <summary>
        /// 加密文字
        /// </summary>
        /// <param name="plainText">要加密的明文</param>
        /// <param name="key">密鑰（可以是 SHA-128, SHA-192, SHA-256）</param>
        /// <param name="iv">初始化向量（必須是 16 字節長）</param>
        /// <returns>Base64 編碼後的密文</returns>
        public static string EncryptString(string plainText, string key, string iv)
        {
            // 檢查初始化向量的有效性（必須是 16 字節長）
            if (iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes long.");

            byte[] keyBytes = ConvertKeyToBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (MemoryStream ms = new MemoryStream())
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                        sw.Flush();
                        cs.FlushFinalBlock();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encryption failed.", ex);
            }
        }

        /// <summary>
        /// 解密文字
        /// </summary>
        /// <param name="cipherText">加密後的密文</param>
        /// <param name="key">密鑰（可以是 SHA-128, SHA-192, SHA-256）</param>
        /// <param name="iv">初始化向量（必須是 16 字節長）</param>
        /// <returns>解密後的明文</returns>
        public static string DecryptString(string cipherText, string key, string iv)
        {
            // 檢查初始化向量的有效性（必須是 16 字節長）
            if (iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes long.");

            byte[] keyBytes = ConvertKeyToBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] buffer = Convert.FromBase64String(cipherText);

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (MemoryStream ms = new MemoryStream(buffer))
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Decryption failed.", ex);
            }
        }

        /// <summary>
        /// 根據密鑰的長度將密鑰轉換為字節數組
        /// </summary>
        /// <param name="key">Base64 格式的密鑰</param>
        /// <returns>對應長度的密鑰字節數組</returns>
        private static byte[] ConvertKeyToBytes(string key)
        {
            // 根據 AES 支持的密鑰長度 (16, 24, 32) 來選擇密鑰長度
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                throw new ArgumentException("Key must be 16, 24, or 32 bytes long.");

            return Encoding.UTF8.GetBytes(key); ;
        }
    }

}
