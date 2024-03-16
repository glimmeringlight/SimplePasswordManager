using System;
using PasswordManager.utils;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace PasswordManager.pages
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            FrameNavigator.NavigateToPage(new HomePage(0));
        }



        private void ConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            SecureString secureString = PasswordInputBox.SecurePassword;
            byte[] raw_random_key = SecurityFunctions.VerifyUserPassword(secureString);
            if (raw_random_key == null)
            {
                // Debug.WriteLine("密码错误");
                PasswordInputBoxReminder.Text = "Wrong password!";
                return;
            }

            // 转到操作界面
            GlobalSettings.raw_random_key = raw_random_key;
            GlobalSettings.logined_state = true;

            FrameNavigator.NavigateToPage(new FunctionPage());
        }


    }
}
