using PasswordManager.utils;
using PasswordManager.windows;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordManager.pages
{
    /// <summary>
    /// FunctionPage.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionPage : Page
    {
        public FunctionPage()
        {
            InitializeComponent();

            listView.ItemsSource = DatabaseManager.passwordList;
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(
                (MainWindow)Window.GetWindow(this));

            changePasswordWindow.Show();
            return;
        }

        private void listview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 获取双击的行
            var listView = sender as ListView;
            if (listView.SelectedItems.Count <=0 )
            {
                return;
            }
            var selectedPasswordRecord = listView.SelectedItems[0] as PasswordRecord;

            EditWindow editwindow = new EditWindow(selectedPasswordRecord);
            editwindow.Show();


        }

        private void ChangeRandomKey_Click(object sender, RoutedEventArgs e)
        {
            FrameNavigator.NavigateToPage(new GenerateNewRandomKeyPage(this));
        }

        private void NewPasswordRecordButton_Click(object sender, RoutedEventArgs e)
        {
            NewPasswordWindow newPasswordWindow = new NewPasswordWindow();
            newPasswordWindow.Show();
        }
    }
}
