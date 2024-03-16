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
using PasswordManager;
using PasswordManager.utils;
using System.Linq;

namespace PasswordManager.windows
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        public PasswordRecord current_pr;
        // delete 确认计数器
        bool confirm_deletion = false;
        public EditWindow(PasswordRecord pr)
        {
            InitializeComponent();
            this.Title = "Edit Password";
            this.ResizeMode = ResizeMode.CanMinimize;
            // 实例化成员变量
            current_pr = pr;
            // 为输入框赋值
            LabelInputBox.Text = pr.Label; 
            UsernameInputBox.Text = pr.Username;
            PasswordInputBox.Text = CryptoAlgorithm.DecryptPassword(
                GlobalSettings.raw_random_key, pr.Password);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // 修改了内容
            string new_label = LabelInputBox.Text;
            string new_username = UsernameInputBox.Text;    
            string new_password = CryptoAlgorithm.EncryptPassword(
                GlobalSettings.raw_random_key, PasswordInputBox.Text);

            // 判断与原来是否相同，不相同则要改数据
            if (new_label == current_pr.Label && new_username == current_pr.Username 
                && new_password == current_pr.Password)
            {
                Close();
                return;
            }

            // 否则，修改数值
            current_pr.Label = new_label;
            current_pr.Username = new_username;
            current_pr.Password = new_password;
            // 将内容同步至Global
            foreach(var record in DatabaseManager.passwordList)
            {
                if (record.ID == current_pr.ID)
                {
                    record.Label = current_pr.Label;
                    record.Username = current_pr.Username;
                    record.Password = current_pr.Password;
                }
            }
            //写入数据库
            DatabaseManager.SyncPasswordListToDatabase();
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!confirm_deletion)
            {
                DeletionCautionReminder.Text = "Confirm deletion? This record will be" +
                    " permanently deleted. Click Delete Button again to delete.";
                confirm_deletion = true;
                return;
            }

            foreach(var item in  DatabaseManager.passwordList)
            {
                if(item.ID == current_pr.ID)
                {
                    DatabaseManager.passwordList.Remove(item);
                    DatabaseManager.SyncPasswordListToDatabase();
                    break;
                }
            }
            Close();
            return;
        }
    }
}
