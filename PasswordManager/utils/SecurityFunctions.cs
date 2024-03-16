using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using PasswordManager;
using System.Security;

namespace PasswordManager.utils
{
    internal static class SecurityFunctions
    {
        public static byte[] VerifyUserPassword(SecureString user_password)
        {
            string unsecureString = ConvertToUnsecureString(user_password);

            // 验证密码的正确性
            var enc_verify_data = DatabaseManager.encrypted_verify_data;
            var encrypted_random_key = DatabaseManager.encrypted_random_key;

            byte[] raw_random_key;
            try
            {
                raw_random_key = CryptoAlgorithm.DecryptRandomKey(unsecureString, encrypted_random_key);
            }
            catch (Exception)
            {
                // Debug.WriteLine("密码错误");
                return null;
            }
            string dec_verify_data = CryptoAlgorithm.DecryptPassword(raw_random_key, enc_verify_data);
            // here, default_password_example should be "success".

            if (dec_verify_data != "success")
            {
                return null;
            }
            
            return raw_random_key;
        }

        public static bool ChangePassword(SecureString new_password)
        {
            string unsecureString = ConvertToUnsecureString(new_password);

            // 获取随机密钥的新密文
            string enc_random_key = CryptoAlgorithm.EncryptRandomKey(unsecureString,
                GlobalSettings.raw_random_key);
            DatabaseManager.setEncryptedKey(enc_random_key, GlobalSettings.raw_random_key);
            return true;
        }

        public static bool isSamePassword(SecureString secure_password_1, SecureString secure_password_2)
        {
            string password_1 = ConvertToUnsecureString(secure_password_1);
            string password_2 = ConvertToUnsecureString(secure_password_2);

            if (password_1 == password_2)
            {
                return true;
            }
            return false;
        }

        public static (byte[], string) GenerateNewRandomKeyPair(SecureString secure_user_password)
        {
            string user_password = ConvertToUnsecureString((SecureString)secure_user_password);
            byte[] new_raw_random_key = CryptoAlgorithm.getRandomKey();
            string new_enc_random_key = CryptoAlgorithm.EncryptRandomKey(user_password, new_raw_random_key);

            return (new_raw_random_key, new_enc_random_key);
        }

        private static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                return string.Empty;

            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(securePassword);
            string unsecurePassword = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
            System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(ptr);

            return unsecurePassword;
        }

        private static void ClearSecureString(SecureString secureString)
        {
            if (secureString != null)
            {
                secureString.Dispose();
            }
        }
    }
}
