using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace localyzer.connect.Views.MainView
{
    [Action(
        "ShowLocalyzerConnectViewPartAction",
        typeof(EditorController),
        Name = "Localyzer Connect",
        Description = "Show Localyzer Connect View Part",
        Icon = "LocalyzerConnect")]
    [ActionLayout(typeof(LocalyzerConnectRibbonGroup), 10, DisplayType.Large)]
    public class ShowLocalyzerConnectViewPartAction : AbstractViewControllerAction<EditorController>
    {
        protected override void Execute()
        {
            var viewPart = SdlTradosStudio.Application.GetController<MainViewPartController>();
            viewPart?.Show();
        }
    }
}
