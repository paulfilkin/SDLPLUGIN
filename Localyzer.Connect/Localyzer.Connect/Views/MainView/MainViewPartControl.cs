using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace localyzer.connect.Views.MainView
{
    public partial class MainViewPartControl : UserControl, IUIControl
    {
        private Label _label;

        public MainViewPartControl()
        {
            InitializeComponent();

            _label = new Label
            {
                Text = "Active segment text will appear here.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            Controls.Add(_label);
        }

        public void UpdateSegmentText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => _label.Text = text));
            }
            else
            {
                _label.Text = text;
            }
        }
    }
}
