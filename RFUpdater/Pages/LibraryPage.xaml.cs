using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        WebClient webClient = new WebClient();
        public GamePage _GamePage;
        public MainWindow _MainWindow;

        Uri GameListFileUri = new Uri("https://drive.google.com/uc?id=1QzOoLrQKW48salKmltEPDAis2Rd_GFz9", UriKind.RelativeOrAbsolute);

        //lists massives
        public List<GameData> ListWithGameData = new List<GameData>();
        GamesInfoClass[] gamesInfoClasses;

        //ints
        public int _Tag;

        Uri ImageSourceUri = new Uri("https://drive.google.com/uc?id=1pbvzQhhskJR8Vi-y-rC9AJ4iI1uylJ5g", UriKind.RelativeOrAbsolute); //RFU logo - https://drive.google.com/uc?id=1vm1sKGFaGSlJWCSaqyiMdR2z62FUTewn https://drive.google.com/file/d/1vm1sKGFaGSlJWCSaqyiMdR2z62FUTewn/view?usp=sharing gamelist - https://drive.google.com/uc?id=1QzOoLrQKW48salKmltEPDAis2Rd_GFz9 https://drive.google.com/uc?id=1Ia1E7q7Hpz-zihtBMxI8jDgGk5tXEk-X
        public LibraryPage(MainWindow mainWindow)
        {
            InitializeComponent();
            gamesInfoClasses = mainWindow.GamesInfoClassList;
            _MainWindow = mainWindow;
            Check();
        }

        public void Check()
        {
            GoogleDiscGamesCheck();
        }

        async void GoogleDiscGamesCheck()
        {
            try
            {
                ListWithGameData.Clear();

                string GameListFilePath = Properties.Settings.Default.AppDataPath + "gamelist.txt";

                if (File.Exists(GameListFilePath))
                {
                    File.Delete(GameListFilePath);
                }

                webClient.DownloadFile(GameListFileUri, GameListFilePath);

                using (StreamReader StreamReader = new StreamReader(GameListFilePath))
                {
                    int LineNum = 0;
                    string[] LineList;
                    string line;
                    string _GameReleaseStatus = "";

                    while ((line = await StreamReader.ReadLineAsync()) != null)
                    {
                        try
                        {
                            if (gamesInfoClasses[LineNum] == null)
                            {
                                gamesInfoClasses[LineNum] = new GamesInfoClass();
                            }
                        }
                        catch
                        {
                            gamesInfoClasses[LineNum] = new GamesInfoClass();
                        }

                        LineList = line.Split('}');
                        if(gamesInfoClasses[LineNum].GameName == null)
                        {
                            gamesInfoClasses[LineNum].GameName = LineList[0];
                        }
                        if (gamesInfoClasses[LineNum].GameName != LineList[0])
                        {
                            gamesInfoClasses[LineNum].GameName = LineList[0];
                        }

                        gamesInfoClasses[LineNum].GamePictureUri = LineList[1];
                        gamesInfoClasses[LineNum].InfoDriveLocationUri = LineList[2];
                        gamesInfoClasses[LineNum].GameReleaseStatus = Convert.ToInt32(LineList[3]);

                        switch(Convert.ToInt32(LineList[3]))
                        {
                            case 0:
                                _GameReleaseStatus = "Soon";
                                break;
                            case 1:
                                _GameReleaseStatus = "Beta";
                                break;
                            case 2:
                                _GameReleaseStatus = "";
                                break;
                        }

                        ListWithGameData.Add(new GameData() { AGameName = gamesInfoClasses[LineNum].GameName, IconSource = new Uri(LineList[1], UriKind.RelativeOrAbsolute), BtnTag = Convert.ToString(gamesInfoClasses[LineNum].Tag), GameReleaseStatus = _GameReleaseStatus });
                        LineNum++;
                    }
                    StreamReader.Dispose();
                    GameItemsControl.ItemsSource = ListWithGameData;
                }
                File.Delete(GameListFilePath);
            }
            catch
            {
                MessageBox.Show("Error: check your internet connection. ", "Error");
                AddTestGame();
            }
        }

        private void AGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Button PressedButton = (Button)sender;
            _Tag = Convert.ToInt32((string)PressedButton.Tag);

            GamesInfoClass _GamesInfoClass = gamesInfoClasses[_Tag];

            string GameInfoPath = Properties.Settings.Default.AppDataPath + "GameInfo.txt";

            try
            {
                if(_GamesInfoClass != null)
                {
                    webClient.DownloadFile(new Uri(_GamesInfoClass.InfoDriveLocationUri, UriKind.RelativeOrAbsolute), GameInfoPath);

                    using (StreamReader StreamReader = new StreamReader(GameInfoPath))
                    {
                        gamesInfoClasses[_Tag].NewGameVersion = new Version(StreamReader.ReadLine());
                        gamesInfoClasses[_Tag].GameDriveLocationUri = StreamReader.ReadLine();
                        StreamReader.Dispose();
                    }
                    File.Delete(GameInfoPath);
                }
                else
                {
                    gamesInfoClasses[_Tag].NewGameVersion = new Version("0.0");
                    gamesInfoClasses[_Tag].GameDriveLocationUri = "";
                }
            }
            catch
            {
                //f
            }

            _MainWindow.GamesInfoClassList[_Tag] = gamesInfoClasses[_Tag];

            _GamePage = new GamePage(_Tag, _MainWindow);
            ((MainWindow)Window.GetWindow(this)).Frame0.Navigate(_GamePage);

        }

        void AddTestGame()
        {
            int LineNum = 0;
            Brush _Brush;

            if (Properties.Settings.Default.ThemeNum == 0)
            {
                _Brush = new SolidColorBrush(Color.FromRgb(97, 214, 200));
            }
            else
            {
                _Brush = new SolidColorBrush(Color.FromRgb(157, 78, 221));
            }

            ListWithGameData.Add(new GameData() { AGameName = "Test game", IconSource = new Uri("https://drive.google.com/uc?id=1NkVH1Zf7WpGQrpI4r5nBz0ZHqsC97BVX", UriKind.RelativeOrAbsolute), BtnTag = Convert.ToString(LineNum), GameReleaseStatus = "T", ReleaseStatusTextBlockBrush = _Brush });
            GameItemsControl.ItemsSource = ListWithGameData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Reload")
            {
                Check();
            }

        }
    }

    public class GameData
    {
        public string AGameName { get; set; }
        public Uri IconSource { get; set; }
        public string BtnTag { get; set; }
        public string GameReleaseStatus { get; set; }
        public Brush ReleaseStatusTextBlockBrush { get; set; }
    }
}
