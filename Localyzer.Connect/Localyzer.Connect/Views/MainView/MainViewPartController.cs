using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

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
        private MainViewPartControl _control;
        private EditorController _editorController;

        protected override IUIControl GetContentControl()
        {
            if (_control == null)
            {
                _control = new MainViewPartControl();
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

        private void UpdateActiveSegment()
        {
            var activeSegment = _editorController?.ActiveDocument?.ActiveSegmentPair;
            if (activeSegment != null)
            {
                var segmentText = activeSegment.Source.ToString(); // or Target.ToString()
                _control?.UpdateSegmentText(segmentText);
            }
            else
            {
                _control?.UpdateSegmentText(string.Empty);
            }
        }

    }
}
