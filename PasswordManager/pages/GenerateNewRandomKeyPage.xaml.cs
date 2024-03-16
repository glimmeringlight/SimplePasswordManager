using PasswordManager.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordManager.pages
{
    /// <summary>
    /// GenerateNewRandomKey.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateNewRandomKeyPage : Page
    {
        private FunctionPage _function_page;
        public GenerateNewRandomKeyPage(FunctionPage function_page)
        {
            InitializeComponent();
            _function_page = function_page;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FrameNavigator.NavigateToPage(_function_page);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SecureString password = PasswordInputBox.SecurePassword;
            byte[] random_key = SecurityFunctions.VerifyUserPassword(password);

            if (random_key == null)
            {
                MessageBlock.Text = "Wrong password!";
                return;
            }

            // 获取新密钥
            (byte[], string)new_random_key_pair = SecurityFunctions.GenerateNewRandomKeyPair(password);
            byte[] new_raw_random_key = new_random_key_pair.Item1;
            string new_enc_random_key = new_random_key_pair.Item2;

            DatabaseManager.setEncryptedKey(new_enc_random_key, new_raw_random_key);

            //所有密码用旧密钥解密再用新密钥加密
            foreach(var item in DatabaseManager.passwordList)
            {
                item.Password = CryptoAlgorithm.EncryptPassword(new_raw_random_key,
                    CryptoAlgorithm.DecryptPassword(GlobalSettings.raw_random_key,item.Password));
            }

            //更新数据库
            DatabaseManager.SyncPasswordListToDatabase();

            //更新GlobalSettings
            GlobalSettings.raw_random_key = new_raw_random_key;

            //完成
            FrameNavigator.NavigateToPage(_function_page);

            return;


        }
    }
}
