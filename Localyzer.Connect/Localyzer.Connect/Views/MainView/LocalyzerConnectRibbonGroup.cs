using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace localyzer.connect.Views.MainView
{
    [RibbonGroup("LocalyzerConnectRibbonGroup", typeof(EditorController), Name = "Lingoport", Description = "Localyzer Connect group")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class LocalyzerConnectRibbonGroup : AbstractRibbonGroup
    {
    }
}
