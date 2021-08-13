using System;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        AboutWindow AboutWindow = new AboutWindow();
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\settings.dat";
        bool LogOutPressed;

        int ThemeNum = Properties.Settings.Default.ThemeNum;

        public SettingsPage()
        {
            InitializeComponent();
            CheckAndSet();
            ChangeTheme();
        }

        void ChangeTheme()
        {
            if(Properties.Settings.Default.ThemeNum == 0)
            {
                LogOutBtn.Style = (Style)FindResource("ButtonStyleGreen");
                AboutBtn.Style = (Style)FindResource("ButtonStyleGreen");
                SaveBtn.Style = (Style)FindResource("ButtonStyleGreen");
                OpenFolderBtn.Style = (Style)FindResource("ButtonStyleGreen");
            }
        }

        void CheckAndSet()
        {
            switch (Properties.Settings.Default.UpdaterLanguage)
            {
                case "en-US":
                    LangBtnEn.IsChecked = true;
                    break;
                case "ru-RU":
                    LangBtnRu.IsChecked = true;
                    break;
                case "cs-CZ":
                    LangBtnCs.IsChecked = true;
                    break;
                case "uk-UK":
                    LangBtnUk.IsChecked = true;
                    break;
            }

            if (Properties.Settings.Default.AutoUpdate == true)
            {
                AutoUpdateBtn.IsChecked = true;
            }

            if (Properties.Settings.Default.SaveFolderPath == @"C:\Games\RFUpdater\")
            {
                SavePathRBC.IsChecked = true;
            }
            else
            {
                SavePathRBD.IsChecked = true;
            }

            switch(Properties.Settings.Default.ThemeNum)
            {
                case 0:
                    ThemeBtnGr.IsChecked = true;
                    break;
                case 1:
                    ThemeBtnPi.IsChecked = true;
                    break;
            }

            if (Properties.Settings.Default.UserAuthorizited == false)
            {
                LogOutBtn.Foreground = Brushes.DarkGray;
                LogOutBtn.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if((string)ClickedButton.Tag == "About")
            {
                AboutWindow.ShowInTaskbar = false;
                AboutWindow.Show();
            }
            else if ((string)ClickedButton.Tag == "LogOut")
            {
                if (LogOutPressed == true)
                {
                    Properties.Settings.Default.UserAuthorizited = true;
                    LogOutBtn.Content = "Log out";
                    LogOutPressed = false;
                }
                else
                {
                    Properties.Settings.Default.UserAuthorizited = false;
                    LogOutBtn.Content = "Cancel";
                    LogOutPressed = true;
                }
            }
            else if ((string)ClickedButton.Tag == "Save")
            {
                //save
                Properties.Settings.Default.Save();

                //restart app
                ((MainWindow)Window.GetWindow(this)).KillNotifyIcon();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else if ((string)ClickedButton.Tag == "OpenFolder")
            {
                Forms.FolderBrowserDialog _FolderBrowserDialog = new Forms.FolderBrowserDialog();

                if (_FolderBrowserDialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.SaveFolderPath = _FolderBrowserDialog.SelectedPath;
                    FolderPathTB.Text = _FolderBrowserDialog.SelectedPath;
                }
            }
        }

        private void OpenFolderBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OpenFolderBtn.Content = "";
        }

        private void OpenFolderBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OpenFolderBtn.Content = "";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton CheckedRadioButton = (RadioButton)sender;
            switch((string)CheckedRadioButton.Tag)
            {
                case "En":
                    Properties.Settings.Default.UpdaterLanguage = "en-US";
                    break;
                case "Ru":
                    Properties.Settings.Default.UpdaterLanguage = "ru-RU";
                    break;
                case "Cs":
                    Properties.Settings.Default.UpdaterLanguage = "cs-CZ";
                    break;
                case "Uk":
                    Properties.Settings.Default.UpdaterLanguage = "uk-UK";
                    break;
                case "Green":
                    ThemeNum = 0;
                    break;
                case "Pink":
                    ThemeNum = 1;
                    break;
                case "C":
                    Properties.Settings.Default.SaveFolderPath = @"C:\Games\RFUpdater\";
                    break;
                case "D":
                    Properties.Settings.Default.SaveFolderPath = @"D:\Games\RFUpdater\";
                    break;
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox CheckedCheckBox = (CheckBox)sender;
            switch((string)CheckedCheckBox.Tag)
            {
                case "StartWithWindows":

                    break;
                case "AutoUpdate":

                    break;
            }
        }
    }
}