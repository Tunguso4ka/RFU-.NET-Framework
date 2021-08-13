using System.Windows;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(RFUpdater.Properties.Settings.Default.UpdaterLanguage);
        }

    }
}
