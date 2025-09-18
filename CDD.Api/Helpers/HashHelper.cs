using System.Security.Cryptography;
using System.Text;

namespace CDD.Api.Helpers
{
    /// <summary>
    /// TODO 密碼雜湊
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// 密碼雜湊 預設 Hash Sha512 
        /// NIST 建議：根據美國國家標準與技術研究所 (NIST) 的建議，密碼雜湊的迭代次數應該隨著硬體性能的增長而逐步增加。
        /// NIST 推薦使用至少 100,000 次的迭代次數，尤其是針對需要長期安全保護的密碼。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="pepper"></param>
        /// <param name="base64Salt"></param>
        /// <param name="iterations"></param>
        /// <param name="hashAlgorithm"></param>
        /// <param name="saltLength"></param>
        /// <param name="pepperLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GenHashedPassword(string password, out string pepper, out string base64Salt, int iterations = 10000,
            HashAlgorithmName hashAlgorithm = default, int saltLength = 32, int pepperLength = 32)
        {
            // 檢查 Hash 演算法和 Hash 長度是否匹配
            HashAlgorithmName _hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;
            if (!IsValidHashAlgorithm(_hashAlgorithm)) { throw new ArgumentException("Invalid hash algorithm specified."); }

            // 產生隨機的 pepper
            pepper = GenerateRandomString(pepperLength);
            // 產生 salt
            byte[] salt = GenerateSalt(saltLength);
            base64Salt = Convert.ToBase64String(salt);

            string base64HashedPassword = HashHelper.HashedPassword(password, pepper, base64Salt, iterations, _hashAlgorithm);
            return base64HashedPassword;
        }

        /// <summary>
        /// 密碼雜湊 預設 Hash Sha512 
        /// NIST 建議：根據美國國家標準與技術研究所 (NIST) 的建議，密碼雜湊的迭代次數應該隨著硬體性能的增長而逐步增加。
        /// NIST 推薦使用至少 100,000 次的迭代次數，尤其是針對需要長期安全保護的密碼。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="pepper"></param>
        /// <param name="base64Salt"></param>
        /// <param name="iterations"></param>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GenHashedPassword(string password, string pepper, string base64Salt, int iterations = 10000, HashAlgorithmName hashAlgorithm = default)
        {
            // 檢查 Hash 演算法
            HashAlgorithmName _hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;
            if (!IsValidHashAlgorithm(_hashAlgorithm)) { throw new ArgumentException("Invalid hash algorithm specified."); }
            // 將結果轉換成 Base64 字串以儲存
            string base64HashedPassword = HashHelper.HashedPassword(password, pepper, base64Salt, iterations, _hashAlgorithm);
            return base64HashedPassword;
        }

        private static string HashedPassword(string password, string pepperString, string base64Salt, int iterations, HashAlgorithmName hashAlgorithm)
        {
            byte[] salt = System.Convert.FromBase64String(base64Salt);
            // 將密碼與 pepper 合併後進行雜湊
            byte[] pepper = Encoding.UTF8.GetBytes(pepperString);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // 合併 password + pepper
            byte[] passwordAndPepper = new byte[passwordBytes.Length + pepper.Length];
            Buffer.BlockCopy(passwordBytes, 0, passwordAndPepper, 0, passwordBytes.Length);
            Buffer.BlockCopy(pepper, 0, passwordAndPepper, passwordBytes.Length, pepper.Length);

            // 將 pepperedPassword 與 salt 合併後進行迭代雜湊
            byte[] hashedPassword = HashPasswordWithSaltAndPepper(passwordAndPepper, salt, iterations, hashAlgorithm);
            // 將結果轉換成 Base64 字串以儲存
            string base64HashedPassword = Convert.ToBase64String(hashedPassword);
            return base64HashedPassword;
        }

        public static byte[] GenerateSalt(int length)
        {
            byte[] salt = new byte[length]; // 建議的 salt 長度為 32 個 byte
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static byte[] HashPasswordWithSaltAndPepper(byte[] password, byte[] salt, int iterations, HashAlgorithmName hashAlgorithm)
        {
            // Recommended action
            // Callers should explicitly specify the iteration count(the default is 1000)
            // and hash algorithm name(the default is HashAlgorithmName.SHA1) via a longer overload.
            int keySize = GetKeySize(hashAlgorithm);
            // 使用指定的哈希算法生成鍵
            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithm))
            {
                return rfc2898.GetBytes(keySize);
            }
        }

        private static bool IsValidHashAlgorithm(HashAlgorithmName algorithm)
        {
            // 支援的算法，這裡可以擴展支援更多的 Hash 演算法
            return algorithm == HashAlgorithmName.SHA256 || algorithm == HashAlgorithmName.SHA384 || algorithm == HashAlgorithmName.SHA512 ||
                   algorithm == HashAlgorithmName.SHA1;

        }

        /// <summary>
        /// 取得演算法長度
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetKeySize(HashAlgorithmName hashAlgorithm)
        {
            int keySize;
            // 根據不同的哈希算法設定輸出的字節長度
            switch (hashAlgorithm)
            {
                case var name when hashAlgorithm == HashAlgorithmName.SHA1:
                    keySize = 20; // SHA-1 的輸出長度為 20 bytes 20/3 = 6...2  (6+1)*4=28
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA256:
                    keySize = 32; // SHA-256 的輸出長度為 32 bytes 32/3=10...2 (10+1)*4=44
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA384:
                    keySize = 48; // SHA-384 的輸出長度為 48 bytes 48/3=16 16*4=64   
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA512:
                    keySize = 64; // SHA-512 的輸出長度為 64 bytes 64/3=21...4 (21+1)*4=88
                    break;
                default:
                    throw new ArgumentException("Unsupported hash algorithm");
            }
            return keySize;
        }

        /// <summary>
        /// 取得演算法長度
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetHashStringBase64Size(HashAlgorithmName hashAlgorithm)
        {
            int keySize;
            // 根據不同的哈希算法設定輸出的字節長度
            switch (hashAlgorithm)
            {
                case var name when hashAlgorithm == HashAlgorithmName.SHA1:
                    keySize = 28; // SHA-1 的輸出長度為 20 bytes 20/3 = 6...2  (6+1)*4=28
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA256:
                    keySize = 44; // SHA-256 的輸出長度為 32 bytes 32/3=10...2 (10+1)*4=44
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA384:
                    keySize = 64; // SHA-384 的輸出長度為 48 bytes 48/3=16 16*4=64   
                    break;
                case var name when hashAlgorithm == HashAlgorithmName.SHA512:
                    keySize = 88; // SHA-512 的輸出長度為 64 bytes 64/3=21...4 (21+1)*4=88
                    break;
                default:
                    throw new ArgumentException("Unsupported hash algorithm");
            }
            return keySize;
        }


        public static string GenerateRandomString(int length, string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var charArray = charSet.Distinct().ToArray();
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = charArray[RandomNumberGenerator.GetInt32(charArray.Length)];
            return new string(result);
        }
    }

}
