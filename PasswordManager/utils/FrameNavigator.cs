using System.Windows.Controls;

namespace PasswordManager.utils
{
    public static class FrameNavigator
    {
        static Frame _frame;

        public static void setFrame(Frame frame)
        {
            _frame = frame;
        }
        public static void NavigateToPage(object obj)
        {
            _frame.Navigate(obj);
        }

    }
}
