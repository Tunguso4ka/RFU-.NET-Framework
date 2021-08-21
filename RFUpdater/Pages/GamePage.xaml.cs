using System;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using locale = RFUpdater.Properties.Lang.Lang;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public MainWindow _MainWindow;

        string ZipPath;
        string GameInfoPath;
        string[] ExeFiles;

        int ThisUserLikeNum = 0;
        int GameStatus;
        int ExeFileNum;

        bool GameIsComingSoon;

        ProgressBar _ProgressBar;
        TextBlock _ProgressTextBox;

        GamesInfoClass _GamesInfoClass;

        public GamePage(int _Tag, MainWindow mainWindow, GamesInfoClass gamesInfoClass)
        {
            InitializeComponent();

            _GamesInfoClass = gamesInfoClass;
            _MainWindow = mainWindow;

            GameNameTextBlock.Text = _GamesInfoClass.GameName;

            if (_GamesInfoClass.GameReleaseStatus == 0)
            {
                GameReleaseStatusTextBlock.Text = "Soon";
                InstallBtn.IsEnabled = false;
                GameIsComingSoon = true;
            }
            else if (_GamesInfoClass.GameReleaseStatus == 1)
            {
                GameReleaseStatusTextBlock.Text = "Beta";
            }
            else
            {
                GameReleaseStatusTextBlock.Text = "";
            }

            if (_GamesInfoClass.CurrentGameVersion == null)
            {
                GameStatus = -2;
            }
            else
            {
                switch (_GamesInfoClass.NewGameVersion.CompareTo(_GamesInfoClass.CurrentGameVersion))
                {
                    case 0:
                        GameStatus = 0; //такая же
                        break;
                    case 1:
                        GameStatus = 1; //новее
                        break;
                    case -1:
                        GameStatus = -1; //старше
                        break;
                }
            }

            //новая и текущая версия вносятся в текстблоки
            if (GameIsComingSoon == false) {
                VersionTextBlock.Text = locale.VersionThis + _GamesInfoClass.CurrentGameVersion;
            }
            if (!_GamesInfoClass.NewGameVersion.Equals(_GamesInfoClass.CurrentGameVersion))
            {
                VersionTextBlock.Text += locale.VersionNew + _GamesInfoClass.NewGameVersion;
            }

            if(_GamesInfoClass.GamePCLocation == null)
            {
                _GamesInfoClass.GamePCLocation = Properties.Settings.Default.SaveFolderPath + _GamesInfoClass.GameName;
                _GamesInfoClass.GamePCLocation = _GamesInfoClass.GamePCLocation.Replace(" ", "");
            }

            ProgressBar0.Visibility = Visibility.Hidden;
            DownSpeedTextBlock.Visibility = Visibility.Hidden;

            if (GameStatus == -2)
            {
                StatusTextBlock.Text = locale.Status + locale.NotInstalled;
                InstallBtn.Content = locale.Install;
                DeleteBtn.Visibility = Visibility.Hidden;
                InstallBtn.Tag = "Install";
            }
            else if (GameStatus == 1)
            {
                StatusTextBlock.Text = locale.Status + locale.UpdateFound;
                InstallBtn.Content = locale.Update;
                DeleteBtn.Visibility = Visibility.Visible;
                InstallBtn.Tag = "Update";
            }
            else
            {
                StatusTextBlock.Text = locale.Status + locale.Installed;
                InstallBtn.Content = locale.Play;
                DeleteBtn.Visibility = Visibility.Visible;
                InstallBtn.Tag = "Play";
            }

            ExeFiles = Directory.GetFiles(_GamesInfoClass.GamePCLocation, "*.exe");
            for (int i = 0; i < ExeFiles.Length; i++)
            {
                exeFilesComboBox.Items.Add(ExeFiles[i]);
            }
            exeFilesComboBox.SelectedItem = 0;

            InfoRead();

            ChangeTheme();

            //MessageBox.Show("" + GameUpdateUri);
        }

        void ChangeTheme()
        {
            if(Properties.Settings.Default.ThemeNum == 0)
            {
                InstallBtn.Style = (Style)FindResource("ButtonStyleGreen");
                DeleteBtn.Style = (Style)FindResource("ButtonStyleGreen");
                LikeBtn.Style = (Style)FindResource("ButtonStyleGreen");
                DisLikeBtn.Style = (Style)FindResource("ButtonStyleGreen");
                GameInfoHideBtn.Style = (Style)FindResource("ButtonStyleGreen");

                GameReleaseStatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(97, 214, 200));
            }
        }

        async void InfoRead()
        {
            try
            {
                string WhatsNewPath = _GamesInfoClass.GamePCLocation + @"\WhatsNew.txt";
                if (GameStatus != 0 && File.Exists(WhatsNewPath))
                {
                    StreamReader _StreamReader = new StreamReader(WhatsNewPath);
                    GameUpdateInfoTextBlock.Text = await _StreamReader.ReadToEndAsync();
                }
            }
            catch 
            {
                GameUpdateInfoTextBlock.Text = "Sorry. No info :(";
            }
        }

        private void Buttons_Click(object sender, RoutedEventArgs e)
        {
            Button _Button = (Button)sender;
            //MessageBox.Show((string)_Button.Tag);
            if ((string)_Button.Tag == "Install")
            {
                //Game Status check
                MessageBox.Show("Installing");
                Installing();
            }
            else if ((string)_Button.Tag == "Play")
            {
                string StartupPath = ExeFiles[0];
                StartupPath = StartupPath.Replace(" ", "");
                if (File.Exists(StartupPath))
                {
                    Process.Start(StartupPath);
                }
                else
                {
                    DeleteGame();
                    MessageBox.Show(_GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName, "Error, Play");
                }
            }
            else if ((string)_Button.Tag == "Delete")
            {
                if (Directory.Exists(_GamesInfoClass.GamePCLocation))
                {
                    Directory.Delete(_GamesInfoClass.GamePCLocation, true);
                }
                DeleteGame();
            }
            else if ((string)_Button.Tag == "Like")
            {
                //like  
                ThisUserLikeNum = 1;
                LikeBtn.Content = "";
                DisLikeBtn.Content = "";
            }
            else if ((string)_Button.Tag == "DisLike")
            {
                //dislike 
                ThisUserLikeNum = 2;
                LikeBtn.Content = "";
                DisLikeBtn.Content = "";
            }
            else if ((string)_Button.Tag == "GameInfo")
            {
                if (GameInfoStackPanel.Visibility == Visibility.Collapsed)
                {
                    GameInfoStackPanel.Visibility = Visibility.Visible;
                    GameInfoHideBtn.ToolTip = "Hide info";
                }
                else
                {
                    GameInfoStackPanel.Visibility = Visibility.Collapsed;
                    GameInfoHideBtn.ToolTip = "Show info";
                }
            }
            else if ((string)_Button.Tag == "Update")
            {
                //Game Status check
                MessageBox.Show("Updating");
                Installing();
            }

        }

        void Installing()
        {
            _ProgressBar = ((MainWindow)Window.GetWindow(this)).MainProgressBar;
            _ProgressTextBox = ((MainWindow)Window.GetWindow(this)).ProgressTextBlock;

            Properties.Settings.Default.Installing = true;
            WebClient WebClient = new WebClient();

            GameInfoPath = Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName + @"\gameinfo.dat";

            try
            {
                if (Directory.Exists(_GamesInfoClass.GamePCLocation))
                {
                    Directory.Delete(_GamesInfoClass.GamePCLocation, true);
                    Directory.CreateDirectory(_GamesInfoClass.GamePCLocation);
                }
                else
                {
                    Directory.CreateDirectory(_GamesInfoClass.GamePCLocation);
                }
            }
            catch
            {
                _GamesInfoClass.GamePCLocation = Properties.Settings.Default.SaveFolderPath + @"\" + _GamesInfoClass.GameName;
                _GamesInfoClass.GamePCLocation = _GamesInfoClass.GamePCLocation.Replace(" ", "");
                Directory.CreateDirectory(_GamesInfoClass.GamePCLocation);
            }

            ZipPath = _GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName + ".zip";
            string GamePath = _GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName + ".exe";

            if(File.Exists(ZipPath))
            {
                File.Delete(ZipPath);
            }

            if (File.Exists(GamePath))
            {
                File.Delete(GamePath);
            }

            Uri UpdateUri = new Uri(_GamesInfoClass.GameDriveLocationUri);
            WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            WebClient.DownloadFileAsync(UpdateUri, ZipPath);

            InstallBtn.IsEnabled = false;
            //ProgressBar0.Visibility = Visibility.Visible;
            _ProgressBar.Visibility = Visibility.Visible;
            _ProgressTextBox.Visibility = Visibility.Visible;
            DownSpeedTextBlock.Visibility = Visibility.Visible;
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //ProgressBar0.Value = e.ProgressPercentage;
            _ProgressBar.Value = e.ProgressPercentage;
            _ProgressTextBox.Text = e.ProgressPercentage + "%";
            DownSpeedTextBlock.Text = "Bytes received: " + e.BytesReceived;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                try
                {
                    MessageBox.Show("Complete");
                    GameStatus = 0;
                    _GamesInfoClass.GameStatus = GameStatus;
                    _GamesInfoClass.CurrentGameVersion = _GamesInfoClass.NewGameVersion;

                    StatusTextBlock.Text = locale.Status + locale.Installed;
                    InstallBtn.Content = locale.Play;
                    InstallBtn.Tag = "Play";

                    ZipFile.ExtractToDirectory(ZipPath, _GamesInfoClass.GamePCLocation);
                    File.Delete(ZipPath);

                    if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName);
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName + @"\Screenshots\");
                    }

                    BinaryWriter BinaryWriter = new BinaryWriter(File.Open(GameInfoPath, FileMode.Create));

                    BinaryWriter.Write(_GamesInfoClass.GameName);
                    BinaryWriter.Write(_GamesInfoClass.GamePictureUri);
                    BinaryWriter.Write(_GamesInfoClass.InfoDriveLocationUri);
                    BinaryWriter.Write(_GamesInfoClass.GameDriveLocationUri);
                    BinaryWriter.Write(_GamesInfoClass.GamePCLocation);
                    BinaryWriter.Write(Convert.ToString(_GamesInfoClass.CurrentGameVersion));
                    BinaryWriter.Write(Convert.ToString(_GamesInfoClass.NewGameVersion));
                    BinaryWriter.Write(_GamesInfoClass.GameStatus);
                    BinaryWriter.Write(_GamesInfoClass.GameReleaseStatus);
                    BinaryWriter.Write(_GamesInfoClass.Tag);

                    _MainWindow.GamesInfoClassList[_GamesInfoClass.Tag] = _GamesInfoClass;

                    BinaryWriter.Dispose();
                }
                catch
                {
                    MessageBox.Show("Error: Can't download this app, try later.", "Error");
                }
            }
            InstallBtn.IsEnabled = true;
            DeleteBtn.Visibility = Visibility.Visible;
            //ProgressBar0.Visibility = Visibility.Hidden;
            _ProgressBar.Visibility = Visibility.Collapsed;
            _ProgressTextBox.Visibility = Visibility.Collapsed;
            DownSpeedTextBlock.Visibility = Visibility.Collapsed;

            Properties.Settings.Default.Installing = false;
            Properties.Settings.Default.Save();
        }

        void DeleteGame()
        {
            GameStatus = -2;
            _GamesInfoClass.GameStatus = GameStatus;
            _GamesInfoClass.CurrentGameVersion = null;

            StatusTextBlock.Text = locale.Status + locale.NotInstalled;
            InstallBtn.Content = locale.Install;
            InstallBtn.Tag = "Install";
            DeleteBtn.Visibility = Visibility.Hidden;

            if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName))
            {
                Directory.Delete(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName, true);
            }
        }

        private void exeFilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExeFileNum = exeFilesComboBox.SelectedIndex;
        }
    }
}
