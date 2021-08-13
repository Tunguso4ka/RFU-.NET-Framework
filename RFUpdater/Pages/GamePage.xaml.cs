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
        string SettingsPath = Properties.Settings.Default.AppDataPath + "gamesonthispc.dat";

        int ThisUserLikeNum = 0;
        int GameStatus;

        bool GameIsComingSoon;

        ProgressBar _ProgressBar;
        TextBlock _ProgressTextBox;

        DirectoryInfo FolderPathDirectory;

        GamesInfoClass _GamesInfoClass;

        public GamePage(int _Tag, MainWindow mainWindow)
        {
            InitializeComponent();

            _GamesInfoClass = mainWindow.GamesInfoClassList[_Tag];
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
            }
            else if (GameStatus == 1)
            {
                StatusTextBlock.Text = locale.Status + locale.UpdateFound;
                InstallBtn.Content = locale.Update;
                DeleteBtn.Visibility = Visibility.Visible;
            }
            else
            {
                StatusTextBlock.Text = locale.Status + locale.Installed;
                InstallBtn.Content = locale.Play;
                DeleteBtn.Visibility = Visibility.Visible;
            }

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
                if (GameStatus == -2)
                {
                    MessageBox.Show("Installing");
                    Installing();
                }
                else if (GameStatus == 0)
                {
                    if (File.Exists(_GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName + ".exe"))
                    {
                        Process.Start(_GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName + ".exe");
                    }
                    else
                    {
                        DeleteGame();
                    }
                }
                else if (GameStatus == 1)
                {
                    MessageBox.Show("Updating");
                    Installing();
                }
            }
            else if ((string)_Button.Tag == "Play")
            {
                string StartupPath = _GamesInfoClass.GamePCLocation + @"\" + _GamesInfoClass.GameName + ".exe";
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

        }

        void Installing()
        {
            _ProgressBar = ((MainWindow)Window.GetWindow(this)).MainProgressBar;
            _ProgressTextBox = ((MainWindow)Window.GetWindow(this)).ProgressTextBlock;

            Properties.Settings.Default.Installing = true;
            WebClient WebClient = new WebClient();

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
                _GamesInfoClass.GamePCLocation = Properties.Settings.Default.SaveFolderPath + _GamesInfoClass.GameName;
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

                    BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
                    int i = 0;
                    while (i != 99)
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

                        if (i == _GamesInfoClass.Tag)
                        {
                            BinaryWriter.Write(
                                _GamesInfoClass.GameName + ";" + //0
                                _GamesInfoClass.GamePictureUri + ";" + //1
                                _GamesInfoClass.InfoDriveLocationUri + ";" + //2
                                _GamesInfoClass.GameDriveLocationUri + ";" + //3
                                _GamesInfoClass.GamePCLocation + ";" + //4
                                _GamesInfoClass.CurrentGameVersion + ";" + //5
                                _GamesInfoClass.NewGameVersion + ";" + //6
                                _GamesInfoClass.GameStatus + ";" + //7
                                _GamesInfoClass.GameReleaseStatus + ";" + //8
                                _GamesInfoClass.Tag //9
                                );

                            _MainWindow.GamesInfoClassList[i] = _GamesInfoClass;
                        }
                        else
                        {
                            GamesInfoClass NotUpdatedGamesInfoClass = _MainWindow.GamesInfoClassList[i];
                            if (NotUpdatedGamesInfoClass == null)
                            {
                                NotUpdatedGamesInfoClass = new GamesInfoClass();
                            }
                            if (NotUpdatedGamesInfoClass.GameName == null)
                            {
                                BinaryWriter.Write(
                                    null + ";" + //0
                                    null + ";" + //1
                                    null + ";" + //2
                                    null + ";" + //3
                                    null + ";" + //4
                                    null + ";" + //5
                                    null + ";" + //6
                                    null + ";" + //7
                                    null + ";" + //8
                                    null //9
                                    );
                            }
                            else
                            {
                                BinaryWriter.Write(
                                NotUpdatedGamesInfoClass.GameName + ";" + //0
                                NotUpdatedGamesInfoClass.GamePictureUri + ";" + //1
                                NotUpdatedGamesInfoClass.InfoDriveLocationUri + ";" + //2
                                NotUpdatedGamesInfoClass.GameDriveLocationUri + ";" + //3
                                NotUpdatedGamesInfoClass.GamePCLocation + ";" + //4
                                NotUpdatedGamesInfoClass.CurrentGameVersion + ";" + //5
                                NotUpdatedGamesInfoClass.NewGameVersion + ";" + //6
                                NotUpdatedGamesInfoClass.GameStatus + ";" + //7
                                NotUpdatedGamesInfoClass.GameReleaseStatus + ";" + //8
                                NotUpdatedGamesInfoClass.Tag //9
                                );
                            }
                            _MainWindow.GamesInfoClassList[i] = NotUpdatedGamesInfoClass;
                        }
                        i++;
                    }

                    if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName);
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + _GamesInfoClass.GameName + @"\Screenshots\");
                    }

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

            BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
            int i = 0;
            while (i != 99)
            {
                if (i == _GamesInfoClass.Tag)
                {
                    BinaryWriter.Write(
                        null + ";" + //0
                        null + ";" + //1
                        null + ";" + //2
                        null + ";" + //3
                        null + ";" + //4
                        null + ";" + //5
                        null + ";" + //6
                        null + ";" + //7
                        null + ";" + //8
                        null //9
                        );

                    _MainWindow.GamesInfoClassList[i] = _GamesInfoClass;
                }
                else
                {
                    GamesInfoClass NotUpdatedGamesInfoClass = _MainWindow.GamesInfoClassList[i];
                    if (NotUpdatedGamesInfoClass == null)
                    {
                        NotUpdatedGamesInfoClass = new GamesInfoClass();
                    }
                    if (NotUpdatedGamesInfoClass.GameName == null)
                    {
                        BinaryWriter.Write(
                            null + ";" + //0
                            null + ";" + //1
                            null + ";" + //2
                            null + ";" + //3
                            null + ";" + //4
                            null + ";" + //5
                            null + ";" + //6
                            null + ";" + //7
                            null + ";" + //8
                            null //9
                            );
                    }
                    else
                    {
                        BinaryWriter.Write(
                        NotUpdatedGamesInfoClass.GameName + ";" + //0
                        NotUpdatedGamesInfoClass.GamePictureUri + ";" + //1
                        NotUpdatedGamesInfoClass.InfoDriveLocationUri + ";" + //2
                        NotUpdatedGamesInfoClass.GameDriveLocationUri + ";" + //3
                        NotUpdatedGamesInfoClass.GamePCLocation + ";" + //4
                        NotUpdatedGamesInfoClass.CurrentGameVersion + ";" + //5
                        NotUpdatedGamesInfoClass.NewGameVersion + ";" + //6
                        NotUpdatedGamesInfoClass.GameStatus + ";" + //7
                        NotUpdatedGamesInfoClass.GameReleaseStatus + ";" + //8
                        NotUpdatedGamesInfoClass.Tag //9
                        );
                    }
                }
                i++;
            }
            BinaryWriter.Dispose();
        }
    }
}
