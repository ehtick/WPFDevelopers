using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ToastExample.xaml 的交互逻辑
    /// </summary>
    public partial class ToastExample : UserControl
    {
        public ToastExample()
        {
            InitializeComponent();
           
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (Enum.TryParse(radioButton.Content.ToString(), out Position position))
                    Toast.SetPosition(position);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "Info":
                    Toast.Push(App.Current.MainWindow, "This is a info toast", ToastImage.Info);
                    break;
                case "Error":
                    Toast.Push("This is a error toast", ToastImage.Error, true);
                    break;
                case "Warning":
                    Toast.Push("This is a warning toast", ToastImage.Warning, true);
                    break;
                case "Success":
                    Toast.Push("This is a question toast", ToastImage.Success);
                    break;
                case "IntPtr":
                    Window parentWindow = Window.GetWindow(this);
                    IntPtr windowHandle = new WindowInteropHelper(parentWindow).Handle;
                    Toast.Push(windowHandle, "This is a Handle toast", ToastImage.Success);
                    break;
                default:
                    Toast.Push("这是一条很长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长消息", ToastImage.Info);
                    break;
            }
        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Toast.Clear();
        }
        private void BtnDesktopClear_Click(object sender, RoutedEventArgs e)
        {
            Toast.ClearDesktop();
        }
        

        private void AddButtonDesktop_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "Info":
                    Toast.PushDesktop("This is a info toast", ToastImage.Info);
                    break;
                case "Error":
                    Toast.PushDesktop("This is a error toast", ToastImage.Error, true);
                    break;
                case "Warning":
                    Toast.PushDesktop("This is a warning toast", ToastImage.Warning, true);
                    break;
                case "Success":
                    Toast.PushDesktop("This is a Success toast", ToastImage.Success);
                    break;
                default:
                    Toast.PushDesktop("这是一条很长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长消息", ToastImage.Info);
                    break;
            }
        }
    }
}
