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

            return _control;
        }

        protected override void Initialize()
        {
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();

            if (_editorController != null)
            {
                _editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
                TryAttachSegmentChanged();
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

        static string GetSegmentUrl(XDocument doc, string markerId)
        {
            XNamespace ns = "urn:oasis:names:tc:xliff:document:1.2";
            XNamespace sdl = "http://sdl.com/FileTypes/SdlXliff/1.0";

            // 1. find the <trans-unit> containing <mrk mid="markerId">
            var tu = doc
                .Descendants(ns + "trans-unit")
                .FirstOrDefault(t => t
                    .Descendants(ns + "mrk")
                    .Any(m => (string)m.Attribute("mid") == markerId)
                );
            if (tu == null)
                return null;

            // 2. get its <sdl:cmt id="..."/>
            var cmtId = tu
                .Elements(sdl + "cmt")
                .Select(c => (string)c.Attribute("id"))
                .FirstOrDefault(id => id != null);
            if (cmtId == null)
                return null;

            // 3. look up the URL in <sdl:cmt-def id="cmtId">
            return doc
                .Descendants(sdl + "cmt-def")
                .Where(cd => (string)cd.Attribute("id") == cmtId)
                .Select(cd => cd
                    .Element(sdl + "Comments")
                    ?.Element(sdl + "Comment")
                    ?.Value
                )
                .FirstOrDefault();
        }

        private void UpdateActiveSegment()
        {
            var activeSegment = _editorController?.ActiveDocument?.ActiveSegmentPair;

            if (activeSegment != null)
            {
                var path = _editorController.ActiveDocument.ActiveFile.LocalFilePath;
                var doc = XDocument.Load(path);
                var url = GetSegmentUrl(doc, activeSegment.Properties.Id.Id);

                var segmentText = activeSegment.Source.ToString(); // or Target.ToString()
                _control?.UpdateSegmentText(segmentText, url);
            }
            else
            {
                _control?.UpdateSegmentText(string.Empty, null);
            }
        }

    }
}
