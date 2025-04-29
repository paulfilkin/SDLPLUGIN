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
        private bool _isWebUrl = false;

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

        //public void Navigate(string contentOrUrl)
        //{
        //    if (WebView2Browser.CoreWebView2 == null)
        //        return;

        //    if (string.IsNullOrWhiteSpace(contentOrUrl))
        //    {
        //        // Show default empty message
        //        var htmlContent = "<html><body><div style='display:flex;justify-content:center;align-items:center;height:100vh;font-size:24px;'>No content</div></body></html>";
        //        WebView2Browser.CoreWebView2.NavigateToString(htmlContent);
        //    }
        //    else if (contentOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        //    {
        //        // It is a URL - navigate to the URL
        //        WebView2Browser.CoreWebView2.Navigate(contentOrUrl);
        //    }
        //    else
        //    {
        //        // It is raw HTML content - render it
        //        WebView2Browser.CoreWebView2.NavigateToString(contentOrUrl);
        //    }
        //}
        public void Navigate(string contentOrUrl)
        {
            if (WebView2Browser.CoreWebView2 == null)
                return;

            if (string.IsNullOrWhiteSpace(contentOrUrl))
            {
                _isWebUrl = false;
                var htmlContent = "<html><body><div style='display:flex;justify-content:center;align-items:center;height:100vh;font-size:24px;'>No content</div></body></html>";
                WebView2Browser.CoreWebView2.NavigateToString(htmlContent);
            }
            else if (contentOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                _isWebUrl = true;
                WebView2Browser.CoreWebView2.Navigate(contentOrUrl);
            }
            else
            {
                _isWebUrl = false;
                WebView2Browser.CoreWebView2.NavigateToString(contentOrUrl);
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

        //private async void WebView2Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        //{
        //    try
        //    {
        //        string script = @"
        //    (function() {
        //        function rescale() {
        //            const page = document.body;
        //            const scale = Math.min(window.innerWidth / 1280, 1);
        //            page.style.transform = 'scale(' + scale + ')';
        //            page.style.transformOrigin = 'top left';
        //            page.style.width = '1280px';
        //        }
        //        window.addEventListener('resize', rescale);
        //        rescale();
        //    })();";

        //        await WebView2Browser.ExecuteScriptAsync(script);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("[WebView2 Injection Error] " + ex.Message);
        //    }
        //}
        private async void WebView2Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (!_isWebUrl)
                    return;

                string script = @"
            (function() {
                function rescale() {
                    const page = document.body;
                    const scale = Math.min(window.innerWidth / 1280, 1);
                    page.style.transform = 'scale(' + scale + ')';
                    page.style.transformOrigin = 'top left';
                    page.style.width = '1280px';
                }
                window.addEventListener('resize', rescale);
                rescale();
            })();";

                await WebView2Browser.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[WebView2 Injection Error] " + ex.Message);
            }
        }



    }
}