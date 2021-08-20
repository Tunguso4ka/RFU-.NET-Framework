using System.Windows;
using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interakční logika pro SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private void ClearSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }

        private void StartSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text != "")
            {

            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text != "")
            {
                ClearSearchBtn.Visibility = Visibility.Visible;
                for (int i = 0; i < 99; i++)
                {
                    //if(((MainWindow)Window.GetWindow(this)).GamesNamesList[i].Contains(SearchTextBox.Text, System.StringComparison.OrdinalIgnoreCase))
                    //{

                    //}
                }
            }
            else
            {
                ClearSearchBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void AGameBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
