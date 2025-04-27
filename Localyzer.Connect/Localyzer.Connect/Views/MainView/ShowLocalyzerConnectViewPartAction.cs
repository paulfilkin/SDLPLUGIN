using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace localyzer.connect.Views.MainView
{
    [Action(
        "ShowLocalyzerConnectViewPartAction",
        Name = "Localyzer Connect",
        Description = "Show Localyzer Connect View Part",
        Icon = "LocalyzerConnect")]
    [ActionLayout(typeof(LocalyzerConnectRibbonGroup), 10, DisplayType.Large)]
    public class ShowLocalyzerConnectViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();

            if (editorController == null || editorController.ActiveDocument == null)
            {
                // No active editor document, do nothing
                return;
            }

            var viewPart = SdlTradosStudio.Application.GetController<MainViewPartController>();
            viewPart?.Show();
        }
    }
}
