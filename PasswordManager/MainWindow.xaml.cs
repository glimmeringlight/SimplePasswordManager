using PasswordManager.pages;
using PasswordManager.utils;
using System.Windows;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "密码管理器";
            this.ResizeMode = ResizeMode.CanMinimize;

            //初始化数据库管理类
            int initializing_state = DatabaseManager.InitializeDatabase();

            //导航到主页面
            FrameNavigator.setFrame(MyFrame);
            FrameNavigator.NavigateToPage(new HomePage(initializing_state));
        }
    }
}