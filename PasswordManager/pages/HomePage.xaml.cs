using PasswordManager.utils;
using System.Windows;
using System.Windows.Controls;

namespace PasswordManager.pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage(int initializing_state)
        {
            InitializeComponent();
            switch (initializing_state)
            {
                case 1:     // 用户首次启动程序
                    {
                        ProgramInfoBlock.Text = "Successfully initialized. Your default password" +
                            " is : admin. Remind to change your password later.";
                        break;
                    }
                default:
                    {
                        ProgramInfoBlock.Text = "Successfully initialized.";
                        break;
                    }

            }
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            FrameNavigator.NavigateToPage(new LoginPage());
        }

    }
}
