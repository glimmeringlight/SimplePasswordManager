using PasswordManager.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace PasswordManager.windows
{
    /// <summary>
    /// NewPasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        public NewPasswordWindow()
        {
            InitializeComponent();
            this.Title = "New Password";
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取用户输入
            string input_label = LabelInputBox.Text;
            string input_username = UsernameInputBox.Text;
            string input_password = PasswordInputBox.Text;

            if (input_label == "" || input_username == "" || input_password == "")
            {
                ReminderBlock.Text = "The input must not be empty!";
                return;
            }

            // 通过验证
            int id = DatabaseManager.getUnusedMinInteger();
            DatabaseManager.passwordList.Add(
                new PasswordRecord(id,input_label,input_username,
                    CryptoAlgorithm.EncryptPassword(GlobalSettings.raw_random_key, input_password)));
            DatabaseManager.SyncPasswordListToDatabase();
            Close();
        }
    }
}
