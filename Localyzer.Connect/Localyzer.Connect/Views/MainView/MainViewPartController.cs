using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace localyzer.connect.Views.MainView
{
    [ViewPart(
        Id = "LocalyzerConnectViewPart",
        Name = "Localyzer Connect",
        Description = "Main View Part for Localyzer Connect",
        Icon = "LocalyzerConnect")]
    public class MainViewPartController : AbstractViewPartController
    {
        private MainViewPartControl _control;

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
            // No special initialization needed here
        }
    }
}
