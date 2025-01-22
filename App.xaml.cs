using System.Configuration;
using System.Data;
using System.Windows;

namespace ExtractTextPdf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Register Syncfusion license
            string licenseKey = ConfigurationManager.AppSettings["SyncfusionLicenseKey"];

            if (!string.IsNullOrEmpty(licenseKey))
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
            else
            {
                MessageBox.Show("Syncfusion license key is missing in App.config.");
            }
        }
    }

}
