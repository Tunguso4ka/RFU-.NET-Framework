using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Win32;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Forms.NotifyIcon notifyIcon;

        //strings
        string SettingsPath;
        string DownGamesPath;
        string RFUUpdateInfoUrl = @"https://drive.google.com/uc?export=download&id=1oKyTppE7V8E-Q0UF0_SXNmW3diQ0QbLJ";
        string RFUUpdateInfoPath;
        const string UriScheme = "rfupdater";
        const string FriendlyName = "RFUpdater Protocol";

        Version NewRFUVersion;

        //pages
        public SettingsPage ASettingsPage;
        public StartPage AStartPage;
        public LibraryPage ALibraryPage;
        public SearchPage ASearchPage;
        public UserPage AUserPage;
        public LoginPage ALoginPage;

        public GamesInfoClass[] GamesInfoClassList = new GamesInfoClass[99];

        public Thread AppThread;

        public MainWindow()
        {
            InitializeComponent();

            CreateNotifyIcon();

            Properties.Settings.Default.AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\";
            Properties.Settings.Default.Save();

            //UpdateCheckerWindow updateCheckerWindow = new UpdateCheckerWindow();
            //updateCheckerWindow.Show();

            if (!Directory.Exists(Properties.Settings.Default.AppDataPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.AppDataPath);
            }
            if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\");
            }
            Checks();

            RegisterUriScheme();

            Frame0.Navigate(AStartPage);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void Pages()
        {
            AStartPage = new StartPage();
            ASettingsPage = new SettingsPage();
            ALibraryPage = new LibraryPage(this);
            ASearchPage = new SearchPage();
            AUserPage = new UserPage();
            ALoginPage = new LoginPage();
        }
        void Checks()
        {

            StringsSet();
            SettingsSearch();
            UpdatesChecking();
            BetaCheck();
            AuthorizCheck();
            ThemeSet();
            Pages();
        }

        void SettingsSearch()
        {
            if (File.Exists(DownGamesPath))
            {
                try
                {
                    GamesInfoClass gamesInfoClass;
                    string[] BinaryReaderOutput;

                    BinaryReader BinaryReader = new BinaryReader(File.OpenRead(DownGamesPath));

                    int LineNum = 0;
                    while (BinaryReader.PeekChar() > -1)
                    {
                        /*
                         * GameName - 0
                         * GamePictureUri - 1
                         * InfoDriveLocationUri - 2
                         * GameDriveLocationUri - 3
                         * GamePCLocation - 4
                         * CurrentGameVersion - 5
                         * NewGameVersion - 6
                         * GameStatus - 7
                         * GameReleaseStatus - 8
                         * Tag - 9
                         */

                        BinaryReaderOutput = BinaryReader.ReadString().Split(';');

                        gamesInfoClass = new GamesInfoClass();
                        gamesInfoClass.GameName = BinaryReaderOutput[0];
                        gamesInfoClass.GamePictureUri = BinaryReaderOutput[1];

                        gamesInfoClass.InfoDriveLocationUri = BinaryReaderOutput[2];
                        gamesInfoClass.GameDriveLocationUri = BinaryReaderOutput[3];
                        gamesInfoClass.GamePCLocation = BinaryReaderOutput[4];

                        try { gamesInfoClass.CurrentGameVersion = new Version(BinaryReaderOutput[5]); }
                        catch { gamesInfoClass.CurrentGameVersion = null; }

                        try { gamesInfoClass.NewGameVersion = new Version(BinaryReaderOutput[6]); }
                        catch { gamesInfoClass.NewGameVersion = null; }

                        try { gamesInfoClass.GameStatus = Convert.ToInt32(BinaryReaderOutput[7]); }
                        catch { gamesInfoClass.GameStatus = 0; }

                        try { gamesInfoClass.GameReleaseStatus = Convert.ToInt32(BinaryReaderOutput[8]); }
                        catch { gamesInfoClass.GameReleaseStatus = 0; }

                        try { gamesInfoClass.Tag = Convert.ToInt32(BinaryReaderOutput[9]); }
                        catch { gamesInfoClass.Tag = LineNum; }

                        GamesInfoClassList[LineNum] = gamesInfoClass;

                        LineNum++;
                    }
                    BinaryReader.Dispose();

                    Properties.Settings.Default.SavedGamesIsReal = true;
                    //MessageBox.Show("", "Yeap");
                }
                catch
                {
                    Properties.Settings.Default.SavedGamesIsReal = false;
                    MessageBox.Show("", "Error SettingsSearch");
                    int i = 0;
                    while (i != 99)
                    {
                        GamesInfoClassList[i] = new GamesInfoClass();
                        i++;
                    }
                }
            }
            else
            {
                Properties.Settings.Default.SavedGamesIsReal = false;
                int i = 0;
                while (i != 99)
                {
                    GamesInfoClassList[i] = new GamesInfoClass();
                    i++;
                }
            }

        }
        void UpdatesChecking()
        {
            try
            {
                if (File.Exists(RFUUpdateInfoPath))
                {
                    File.Delete(RFUUpdateInfoPath);
                }
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(RFUUpdateInfoUrl, RFUUpdateInfoPath);
                using (StreamReader StreamReader = new StreamReader(RFUUpdateInfoPath))
                {
                    NewRFUVersion = new Version(StreamReader.ReadLine());
                    StreamReader.Dispose();
                }
                File.Delete(RFUUpdateInfoPath);

                switch (NewRFUVersion.CompareTo(Assembly.GetExecutingAssembly().GetName().Version))
                {
                    case 0:
                         //такая же
                        break;
                    case 1:
                        if (File.Exists(Properties.Settings.Default.AppDataPath + "RFUI.exe"))
                        {
                            Process.Start(Properties.Settings.Default.AppDataPath + "RFUI.exe");
                        }
                         //новее
                        break;
                }
            }
            catch
            {

            }
        }
        void BetaCheck()
        {
            //Properties.Settings.Default.IsBetaOn
            if(Properties.Settings.Default.IsBetaOn == true)
            {
                UserBtn.Visibility = Visibility.Visible;
            }
        }
        void AuthorizCheck()
        {
            //Properties.Settings.Default.UserAuthorizited
            if (Properties.Settings.Default.UserAuthorizited == true)
            {
                UserBtn.Content = "";
                UserBtn.ToolTip = Properties.Settings.Default.UserName;
            }
        }
        void ThemeSet()
        {
            if (Properties.Settings.Default.ThemeNum == 0)
            {
                WindowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 69, 65));
                FirstGrid.Background = new SolidColorBrush(Color.FromRgb(65, 139, 130));
                SecondGrid.Background = new SolidColorBrush(Color.FromRgb(79, 168, 158));

                MenuBtn.Style = (Style)FindResource("ButtonStyleGreen");
                LibraryBtn.Style = (Style)FindResource("ButtonStyleGreen");
                UserBtn.Style = (Style)FindResource("ButtonStyleGreen");
                MinimBtn.Style = (Style)FindResource("ButtonStyleGreen");
                CloseBtn.Style = (Style)FindResource("ButtonStyleGreen");
            }
        }
        void StringsSet()
        {
            //Properties.Settings.Default.AppDataPath
            SettingsPath = Properties.Settings.Default.AppDataPath + "settings.dat";
            RFUUpdateInfoPath = Properties.Settings.Default.AppDataPath + "RFUV.txt";
            DownGamesPath = Properties.Settings.Default.AppDataPath + "gamesonthispc.dat";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if((string)ClickedButton.Tag == "Settings")
            {
                Frame0.Navigate(ASettingsPage);
                LibraryBtn.Background = Brushes.Transparent;
                SettingsBtn.Background = new SolidColorBrush(Color.FromRgb(36, 00, 70));
                StoreBtn.Background = Brushes.Transparent;
                MenuBtn.Background = Brushes.Transparent;
            }
            else if ((string)ClickedButton.Tag == "Library")
            {
                ALibraryPage.Check();
                Frame0.Navigate(ALibraryPage);

                LibraryBtn.Background = new SolidColorBrush(Color.FromRgb(36,00,70));
                SettingsBtn.Background = Brushes.Transparent;
                StoreBtn.Background = Brushes.Transparent;
                MenuBtn.Background = Brushes.Transparent;
            }
            else if ((string)ClickedButton.Tag == "Close")
            {
                if (Properties.Settings.Default.Installing == false)
                {
                    this.WindowState = WindowState.Minimized;
                    this.ShowInTaskbar = false;
                }
                else
                {
                    this.WindowState = WindowState.Minimized;
                    MessageBox.Show("You can't close Updater while it doing its work.");
                }
            }
            else if ((string)ClickedButton.Tag == "Minimize")
            {
                this.WindowState = WindowState.Minimized;
            }
            else if ((string)ClickedButton.Tag == "Maximize")
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    ClickedButton.Content = "";
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Maximized;
                    ClickedButton.Content = "";
                    this.WindowStyle = WindowStyle.None;
                }
            }
            else if ((string)ClickedButton.Tag == "Menu")
            {
                Frame0.Navigate(AStartPage);
                LibraryBtn.Background = Brushes.Transparent;
                SettingsBtn.Background = Brushes.Transparent;
                StoreBtn.Background = Brushes.Transparent;
                MenuBtn.Background = new SolidColorBrush(Color.FromRgb(36, 00, 70));
            }
            else if ((string)ClickedButton.Tag == "User")
            {
                if (Properties.Settings.Default.UserAuthorizited == true)
                {
                    Frame0.Navigate(AUserPage);
                }
                else
                {
                    Frame0.Navigate(ALoginPage);
                }
            }
            else if ((string)ClickedButton.Tag == "Back")
            {
                if(Frame0.CanGoBack)
                {
                    Frame0.GoBack();
                }
            }
        }

        private void CreateNotifyIcon()
        {

            notifyIcon = new Forms.NotifyIcon(new Container());
            Forms.ContextMenuStrip _ContextMenuStrip = new Forms.ContextMenuStrip();

            Forms.ToolStripMenuItem _StripMenuItemAppName = new Forms.ToolStripMenuItem();

            _ContextMenuStrip.Items.AddRange(
                new Forms.ToolStripMenuItem[]
                {
                    _StripMenuItemAppName,
                    new Forms.ToolStripMenuItem("Main page", null, new EventHandler(StartPageClicked)),
                    new Forms.ToolStripMenuItem("Store", null, new EventHandler(LibraryPageClicked)),
                    new Forms.ToolStripMenuItem("Library", null, new EventHandler(LibraryPageClicked)),
                    new Forms.ToolStripMenuItem("Exit", null, new EventHandler(ExitClicked))
                }
            );

            _StripMenuItemAppName.Text = "RFUpdater";
            _StripMenuItemAppName.Enabled = false;
            _StripMenuItemAppName.Image = Properties.Resources.rfulogo0525;

            notifyIcon.Icon = Properties.Resources.rfulogo0525ico;
            notifyIcon.ContextMenuStrip = _ContextMenuStrip;
            notifyIcon.Text = "RFUpdater";
            notifyIcon.Visible = true;
            notifyIcon.MouseDown += new Forms.MouseEventHandler(NotifyIconClicked);
            
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Maximized;
                MaximizeBtn.Content = "";
                this.WindowStyle = WindowStyle.None;
            }
        }

        public static void RegisterUriScheme()
        {
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
            {
                // Replace typeof(App) by the class that contains the Main method or any class located in the project that produces the exe.
                // or replace typeof(App).Assembly.Location by anything that gives the full path to the exe
                string applicationLocation = typeof(App).Assembly.Location;

                key.SetValue("", "URL:" + FriendlyName);
                key.SetValue("URL Protocol", "");

                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("", applicationLocation + ",1");
                }

                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                }
            }
        }

        [STAThread]
        private void NotifyIconClicked(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                Frame0.Content = AStartPage;
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }
        private void StartPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = AStartPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }
        private void LibraryPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = ALibraryPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ExitClicked(object sender, EventArgs e)
        {
            KillNotifyIcon();
            this.Close();
            Application.Current.Shutdown();
        }

        public void KillNotifyIcon()
        {
            notifyIcon.Dispose();
        }

        private void Frame0_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if(Frame0.CanGoBack)
            {
                BackBtn.Visibility = Visibility.Visible;
            }
            else
            {
                BackBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
