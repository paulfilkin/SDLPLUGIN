using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using Localyzer.Connect.Views;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;


namespace localyzer.connect.Views.MainView
{
    [ViewPart(
        Id = "LocalyzerConnectViewPart",
        Name = "Localyzer Connect",
        Description = "Main View Part for Localyzer Connect",
        Icon = "LocalyzerConnect_Icon")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(EditorController))]
    public class MainViewPartController : AbstractViewPartController
    {
        private MainViewPart _control;
        private EditorController _editorController;
        private bool _isLrmIncontextFile;

        protected override IUIControl GetContentControl()
        {
            if (_control == null)
            {
                _control = new MainViewPart();
            }

            return _control;
        }

        protected override void Initialize()
        {
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();

            if (_editorController != null)
            {
                _editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
                TryAttachSegmentChanged();
                // Do not call UpdateActiveSegment here; wait until a segment is active

                foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                {
                    System.Diagnostics.Debug.WriteLine($"[RESOURCE] {name}");
                }

            }
        }

        private void EditorController_ActiveDocumentChanged(object sender, EventArgs e)
        {
            TryAttachSegmentChanged();
        }

        private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
        {
            UpdateActiveSegment();
        }

        private void TryAttachSegmentChanged()
        {
            if (_editorController?.ActiveDocument != null)
            {
                _editorController.ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
                _editorController.ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;

                // Manually trigger in case segment is already selected
                UpdateActiveSegment();
            }
        }

        private async void UpdateActiveSegment()
        {
            // Safe loop: wait for Editor, Document, Segment, and WebView2
            for (int i = 0; i < 20; i++) // Try for 2 seconds max (20 * 100ms)
            {
                var activeDocument = _editorController?.ActiveDocument;
                var activeSegment = activeDocument?.ActiveSegmentPair;

                if (activeDocument != null && activeSegment != null && _control?.WebView2Browser?.CoreWebView2 != null)
                {
                    await ProceedWithNavigation(activeDocument, activeSegment);
                    return;
                }

                await Task.Delay(100); // Wait 100ms and try again
            }

            // If after 2 seconds we still have nothing, show empty
            await _control.EnsureBrowserIsLoaded();
            _control?.Navigate(null);
        }

        private async Task ProceedWithNavigation(IStudioDocument activeDocument, ISegmentPair activeSegment)

        {
            var path = activeDocument?.ActiveFile?.LocalFilePath;

            if (string.IsNullOrEmpty(path) || !path.EndsWith(".sdlxliff", StringComparison.OrdinalIgnoreCase))
            {
                await _control.EnsureBrowserIsLoaded();
                _control?.Navigate(null);
                return;
            }

            try
            {
                var doc = XDocument.Load(path);
                var fileTypeIdElement = doc
                    .Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "filetype-id");

                var fileTypeId = fileTypeIdElement?.Value ?? string.Empty;
                _isLrmIncontextFile = fileTypeId.IndexOf("LRM_INCONTEXT", StringComparison.OrdinalIgnoreCase) >= 0;

                await _control.EnsureBrowserIsLoaded();

                if (_isLrmIncontextFile)
                {
                    var url = GetSegmentUrl(doc, activeSegment.Properties.Id.Id);
                    _control?.Navigate(url);
                }
                else
                {
                    _control?.Navigate(GetEmbeddedLocalyzerHtml());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to parse sdlxliff: {ex.Message}");
                await _control.EnsureBrowserIsLoaded();
                _control?.Navigate(null);
            }
        }


        private static string GetSegmentUrl(XDocument doc, string markerId)
        {
            XNamespace ns = "urn:oasis:names:tc:xliff:document:1.2";
            XNamespace sdl = "http://sdl.com/FileTypes/SdlXliff/1.0";

            var tu = doc
                .Descendants(ns + "trans-unit")
                .FirstOrDefault(t => t
                    .Descendants(ns + "mrk")
                    .Any(m => (string)m.Attribute("mid") == markerId)
                );

            if (tu == null)
                return null;

            var cmtId = tu
                .Elements(sdl + "cmt")
                .Select(c => (string)c.Attribute("id"))
                .FirstOrDefault();

            if (cmtId == null)
                return null;

            var url = doc
                .Descendants(sdl + "cmt-def")
                .Where(cd => (string)cd.Attribute("id") == cmtId)
                .Select(cd => cd
                    .Element(sdl + "Comments")
                    ?.Element(sdl + "Comment")
                    ?.Value)
                .FirstOrDefault();

            return url;
        }

        private static string GetEmbeddedLocalyzerHtml()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith("getLocalyzer.html", StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                    return null;

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load embedded getLocalyzer.html: {ex.Message}");
                return null;
            }
        }
    }
}