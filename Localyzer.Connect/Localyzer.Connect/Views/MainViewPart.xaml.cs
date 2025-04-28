using Microsoft.Web.WebView2.Wpf;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Net;

namespace Localyzer.Connect.Views
{
    public partial class MainViewPart : UserControl, IUIControl
    {
        private WebView2 _browser;

        public MainViewPart()
        {
            InitializeComponent();

            _browser = new WebView2();
            _browser.HorizontalAlignment = HorizontalAlignment.Stretch;
            _browser.VerticalAlignment = VerticalAlignment.Stretch;
            _browser.Margin = new Thickness(0);

            RootGrid.Children.Add(_browser);
        }

        public void Dispose()
        {
            _browser?.Dispose();
        }

        public async void UpdateSegmentText(string segmentText, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    // Ensure WebView2 is initialized before setting the URL
                    await _browser.EnsureCoreWebView2Async();
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] WebView2 Initialized, rendering URL: {url}");

                    // Now, set the URL in the WebView2 control
                    _browser.Source = new Uri(url);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] WebView2 initialization failed: {ex.Message}");
                }
            }
            else
            {
                _browser.Source = null; // Optional: Show a default page or message if URL is empty
            }
        }

    }
}
