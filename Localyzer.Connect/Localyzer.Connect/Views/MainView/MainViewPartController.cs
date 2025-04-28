using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using Localyzer.Connect.Views;
using System.Xml.Linq;
using System.Linq;

namespace localyzer.connect.Views.MainView
{
    [ViewPart(
        Id = "LocalyzerConnectViewPart",
        Name = "Localyzer Connect",
        Description = "Main View Part for Localyzer Connect",
        Icon = "LocalyzerConnect")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(EditorController))]
    public class MainViewPartController : AbstractViewPartController
    {
        private MainViewPart _control;
        private EditorController _editorController;

        protected override IUIControl GetContentControl()
        {
            if (_control == null)
            {
                _control = new MainViewPart();
            }

            return (IUIControl)_control;
        }

        protected override void Initialize()
        {
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();

            if (_editorController != null)
            {
                _editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
                TryAttachSegmentChanged();

                // Call UpdateActiveSegment immediately when the document is first opened
                UpdateActiveSegment();
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

                // Immediately update the current active segment when opening the document
                UpdateActiveSegment();
            }
        }

        //static string GetSegmentUrl(XDocument doc, string markerId)
        //{
        //    XNamespace ns = "urn:oasis:names:tc:xliff:document:1.2";
        //    XNamespace sdl = "http://sdl.com/FileTypes/SdlXliff/1.0";

        //    // 1. find the <trans-unit> containing <mrk mid="markerId">
        //    var tu = doc
        //        .Descendants(ns + "trans-unit")
        //        .FirstOrDefault(t => t
        //            .Descendants(ns + "mrk")
        //            .Any(m => (string)m.Attribute("mid") == markerId)
        //        );
        //    if (tu == null)
        //        return null;

        //    // 2. get its <sdl:cmt id="..."/>
        //    var cmtId = tu
        //        .Elements(sdl + "cmt")
        //        .Select(c => (string)c.Attribute("id"))
        //        .FirstOrDefault(id => id != null);
        //    if (cmtId == null)
        //        return null;

        //    // 3. look up the URL in <sdl:cmt-def id="cmtId">
        //    return doc
        //        .Descendants(sdl + "cmt-def")
        //        .Where(cd => (string)cd.Attribute("id") == cmtId)
        //        .Select(cd => cd
        //            .Element(sdl + "Comments")
        //            ?.Element(sdl + "Comment")
        //            ?.Value
        //        )
        //        .FirstOrDefault();
        //}

        static string GetSegmentUrl(XDocument doc, string markerId)
        {
            XNamespace ns = "urn:oasis:names:tc:xliff:document:1.2";
            XNamespace sdl = "http://sdl.com/FileTypes/SdlXliff/1.0";

            // Debugging: Log the markerId we're searching for
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Searching for markerId: {markerId}");

            // 1. Find the <trans-unit> containing <mrk mid="markerId">
            var tu = doc
                .Descendants(ns + "trans-unit")
                .FirstOrDefault(t => t
                    .Descendants(ns + "mrk")
                    .Any(m => (string)m.Attribute("mid") == markerId)
                );

            if (tu == null)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] No trans-unit found for markerId: {markerId}");
                return null;
            }

            // 2. Get the <sdl:cmt> id from the <trans-unit>
            var cmtId = tu
                .Elements(sdl + "cmt")
                .Select(c => (string)c.Attribute("id"))
                .FirstOrDefault(id => id != null);

            if (cmtId == null)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] No cmtId found in trans-unit.");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Found cmtId: {cmtId}");

            // 3. Lookup the URL in <sdl:cmt-def id="cmtId">
            var url = doc
                .Descendants(sdl + "cmt-def")
                .Where(cd => (string)cd.Attribute("id") == cmtId)
                .Select(cd => cd
                    .Element(sdl + "Comments")
                    ?.Element(sdl + "Comment")
                    ?.Value
                )
                .FirstOrDefault();

            // Debugging: Log the URL result
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Found URL: {url}");

            return url;
        }


        private void UpdateActiveSegment()
        {
            var activeSegment = _editorController?.ActiveDocument?.ActiveSegmentPair;

            if (activeSegment != null)
            {
                var path = _editorController.ActiveDocument.ActiveFile.LocalFilePath;
                var doc = XDocument.Load(path);
                var url = GetSegmentUrl(doc, activeSegment.Properties.Id.Id);  // Get the URL based on the segment ID

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Active segment URL: {url}");

                _control?.UpdateSegmentText(null, url);  // Only pass the URL to WebView2 (no need for segmentText)
            }
            else
            {
                _control?.UpdateSegmentText(string.Empty, null);
            }
        }

    }
}
