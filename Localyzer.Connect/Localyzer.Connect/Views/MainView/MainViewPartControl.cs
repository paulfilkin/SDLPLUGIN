using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace localyzer.connect.Views.MainView
{
    public partial class MainViewPartControl : UserControl, IUIControl
    {
        public MainViewPartControl()
        {
            InitializeComponent();

            var label = new Label
            {
                Text = "Hello from Localyzer Connect!",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Controls.Add(label);
        }
    }
}
