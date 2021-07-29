using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace RFUpdater
{
    /// <summary>
    /// Interakční logika pro LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr FindWindow(String ClassName, String WindowName);

        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        public LoadingWindow()
        {
            InitializeComponent();

            //поиск окна по заголовку
            IntPtr hWnd = FindWindow(null, "RFUpdater");
            if (hWnd.ToInt32() > 0)
            {
                SetForegroundWindow(hWnd);
                this.Close();
            }
            else
            {
                OpenMainWindow();
            }
        }


        public void OpenMainWindow()
        {
            MainWindow _MainWindow = new MainWindow();
            _MainWindow.Show();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
