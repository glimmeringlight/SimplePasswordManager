using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.utils
{
    public class MyCustomException : Exception
    {
        // 自定义异常类的构造函数
        public MyCustomException(string message) : base(message)
        {
        }
    }

    public class CryptoAlgorithm
    {
        static public byte[] ComputeSha256Hash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        public static byte[] EncryptMD5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return s;
        }

        static public byte[] getRandomKey()
        {
            // 生成随机的32字节密钥 
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] key = new byte[16];
            rng.GetBytes(key);
            Debug.WriteLine(BytesToHexString(key));
            return key;
        }
        static public byte[] HexStringToBytes(string hexString)
        {
            string[] hexValues = hexString.Split('-');
            byte[] bytes = new byte[hexValues.Length];

            for (int i = 0; i < hexValues.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValues[i], 16);
            }

            return bytes;
        }

        static public string BytesToHexString(byte[] bytes)
        {
            string hexString = BitConverter.ToString(bytes);
            return hexString;
        }

        /// <summary>
        /// 加密
        /// IV等于Key且Key为string，明文为bytes
        /// </summary>
        /// <param name="user_key">密钥</param>
        /// <param name="random_key">原文</param>
        /// <returns>密文(Base64字符串)</returns>
        public static string EncryptRandomKey(string user_key, byte[] random_key)
        {
            if (user_key == null)
                throw new ArgumentNullException(nameof(user_key));
            if (random_key == null)
                throw new ArgumentNullException(nameof(random_key));
            return Convert.ToBase64String(BasicEncrypt(EncryptMD5(user_key), EncryptMD5(user_key), BytesToHexString(random_key)));
        }

        public static byte[] DecryptRandomKey(string user_key, string enc_random_key)
        {
            if (user_key == null)
                throw new ArgumentException(nameof(user_key));
            if (enc_random_key == null)
                throw new ArgumentException(nameof(enc_random_key));
            try
            {
                return HexStringToBytes(BasicDecrypt(EncryptMD5(user_key), EncryptMD5(user_key),
                Convert.FromBase64String(enc_random_key)));
            }
            catch (CryptographicException)
            {
                throw;
            }
        }

        public static string EncryptPassword(byte[] random_key, string password)
        {
            if (random_key == null)
                throw new ArgumentException(nameof(random_key));
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            return Convert.ToBase64String(BasicEncrypt(random_key, random_key, password));
        }

        public static string DecryptPassword(byte[] random_key, string enc_password)
        {
            if (random_key == null)
                throw new ArgumentNullException(nameof(random_key));
            if (enc_password == null)
                throw new ArgumentNullException(nameof(enc_password));

            try
            {
                return BasicDecrypt(random_key, random_key, Convert.FromBase64String(enc_password));
            }
            catch (CryptographicException) {
                return null;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="rgbKey">密钥</param>
        /// <param name="rgbIV">初始化向量</param>
        /// <param name="sourceText">原文</param>
        /// <returns>密文</returns>
        public static byte[] BasicEncrypt(byte[] rgbKey, byte[] rgbIV, string sourceText)
        {
            if (rgbKey == null)
                throw new ArgumentNullException(nameof(rgbKey));
            if (rgbIV == null)
                throw new ArgumentNullException(nameof(rgbIV));
            if (sourceText == null)
                throw new ArgumentNullException(nameof(sourceText));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                using (ICryptoTransform transform = aes.CreateEncryptor(rgbKey, rgbIV))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(sourceText);
                    streamWriter.Flush();
                }

                return memoryStream.ToArray();
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="rgbKey">密钥</param>
        /// <param name="rgbIV">初始化向量</param>
        /// <param name="cipherStream">密文</param>
        /// <returns>原文</returns>
        public static string BasicDecrypt(byte[] rgbKey, byte[] rgbIV, byte[] cipherBuffer)
        {
            if (rgbKey == null)
                throw new ArgumentNullException(nameof(rgbKey));
            if (rgbIV == null)
                throw new ArgumentNullException(nameof(rgbIV));
            if (cipherBuffer == null)
                throw new ArgumentNullException(nameof(cipherBuffer));

            using (MemoryStream cipherStream = new MemoryStream(cipherBuffer))
            using (Aes aes = Aes.Create())
            using (ICryptoTransform transform = aes.CreateDecryptor(rgbKey, rgbIV))
            using (CryptoStream cryptoStream = new CryptoStream(cipherStream, transform, CryptoStreamMode.Read))
            using (StreamReader streamReader = new StreamReader(cryptoStream))
            {
                try
                {
                    return streamReader.ReadToEnd();
                }
                catch (CryptographicException)
                {
                    throw;
                }
            }
        }
    }
}
