using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RFUpdater
{
    /// <summary>
    /// Interakční logika pro AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            AboutTextBox.Text = "About: \n\nRF Updater (.NET Framework) \nVersion: " + Assembly.GetExecutingAssembly().GetName().Version;
            if (Properties.Settings.Default.IsBetaOn == true)
            {
                AppNameTextBlock.Text = "RFU β";
                AboutTextBox.Text += "\nBeta: on";
            }
            if (Properties.Settings.Default.UserAuthorizited == true)
            {
                AboutTextBox.Text += "\nUser login: y\nUsername: " + Properties.Settings.Default.UserName;
            }
            else
            {
                AboutTextBox.Text += "\nUser login: n";
            }
            AboutTextBox.Text += "\nMy twitter: https://github.com/tunguso4ka \nMy github: https://twitter.com/tunguso4ka \nI <3 Stef \n\nThank you very much for using <3";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
