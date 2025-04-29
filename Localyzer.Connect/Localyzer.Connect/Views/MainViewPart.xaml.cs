using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Localyzer.Connect.Views
{
    public partial class MainViewPart : UserControl, IUIControl
    {
        public MainViewPart()
        {
            InitializeComponent();
        }

        private CoreWebView2Environment Environment { get; set; }

        public void Dispose()
        {
            WebView2Browser?.Dispose();
        }

        public async Task EnsureBrowserIsLoaded()
        {
            await WebView2Browser.EnsureCoreWebView2Async(Environment);
        }

        public void Navigate(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                var htmlContent = "<html><body><div style='display:flex;justify-content:center;align-items:center;height:100vh;font-size:24px;'>No content</div></body></html>";
                WebView2Browser.CoreWebView2?.NavigateToString(htmlContent);
            }
            else
            {
                WebView2Browser.CoreWebView2?.Navigate(url);
            }
        }

        private async Task InitializeWebView()
        {
            if (WebView2Browser.CoreWebView2 is null)
            {
                var userDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);
                var options = new CoreWebView2EnvironmentOptions { AllowSingleSignOnUsingOSPrimaryAccount = true };
                Environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

                WebView2Browser.CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = userDataFolder
                };

                await EnsureBrowserIsLoaded();
            }

            Navigate(null);
        }

        private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await InitializeWebView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}