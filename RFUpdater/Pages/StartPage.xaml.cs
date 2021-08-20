using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public List<NewsList> ListWithNewsData = new List<NewsList>();

        public StartPage()
        {
            InitializeComponent();
            CreateTestNews();
        }

        void CreateTestNews()
        {
            ListWithNewsData.Add(
                new NewsList() 
                { 
                    NewsText = "Welcome to RFUpdater.\nThere you can download and update games.\nThere can be a many bugs, but im trying to fix it.\nThanks for using!", 
                    IconSource = new Uri("https://drive.google.com/uc?id=1snwcZsH8Mu4nucVT4PqWI18F9UenlhdL", UriKind.RelativeOrAbsolute), 
                    BtnTag = Convert.ToString(0) 
                }
                );
            ListWithNewsData.Add(
                new NewsList()
                {
                    NewsText = "Random Fights - is a first and main RFUpdater game. \n I love this game and i love u too! \n Have a nice day!",
                    IconSource = new Uri("https://drive.google.com/uc?id=1n63bM-yK1YLPSEyR3ehhq3GfSt2Tj1S7", UriKind.RelativeOrAbsolute),
                    BtnTag = Convert.ToString(0)
                }
                );
            NewsItemsControl.ItemsSource = ListWithNewsData;
            //https://drive.google.com/file/d/1snwcZsH8Mu4nucVT4PqWI18F9UenlhdL/view?usp=sharing
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

    public class NewsList
    {
        public string NewsText { get; set; }
        public Uri IconSource { get; set; }
        public string BtnTag { get; set; }
    }
}
