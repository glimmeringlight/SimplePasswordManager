using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security;
using PasswordManager.utils;

namespace PasswordManager.windows
{
    /// <summary>
    /// ChangePasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        bool verify_change_password = false;
        MainWindow _mainwindow;
        public ChangePasswordWindow(MainWindow mainwindow)
        {
            InitializeComponent();
            _mainwindow = mainwindow;
            this.Title = "Change your password";
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            return;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SecureString old_password = OldPasswordInputBox.SecurePassword;
            SecureString new_password = NewPasswordInputBox.SecurePassword;
            SecureString verify_password = VerifyPasswordInputBox.SecurePassword;

            byte[] raw_random_key = SecurityFunctions.VerifyUserPassword(old_password);
            if (raw_random_key == null)
            {
                ReminderMessageBlock.Text = "Wrong old password!";
                return;
            }
            if (!SecurityFunctions.isSamePassword(new_password, verify_password))
            {
                ReminderMessageBlock.Text = "Two different new passwords! Please Check agagin!";
                return;
            }

            // 通过检查
            if (!verify_change_password)
            {
                ReminderMessageBlock.Text = "Your password will be changed! Click confirm button to continue.";
                verify_change_password=true;
                return;
            }

            bool state = SecurityFunctions.ChangePassword(new_password);
            if (state)
            {
                this._mainwindow.Close();
                Close();
                return;
            }
        }
    }
}
