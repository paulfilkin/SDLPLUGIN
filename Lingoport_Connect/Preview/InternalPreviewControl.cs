using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Diagnostics;

namespace Lingoport.LocalyzerConnect.Preview
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class InternalPreviewControl : UserControl
    {
        string _activeSegId = string.Empty;
        string _jumpparagraphID = string.Empty;
        string _jumpsegmentID = string.Empty;
        bool _segmentSelectedFromBrowser = false;

        public event PreviewControlHandler WindowSelectionChanged;

        public InternalPreviewControl()
        {
            Debug.WriteLine(">>> InternalPreviewControl: Constructor called");
            InitializeComponent();
            webBrowserControl.AllowWebBrowserDrop = false;
            webBrowserControl.IsWebBrowserContextMenuEnabled = false;
            webBrowserControl.WebBrowserShortcutsEnabled = false;
            webBrowserControl.ScriptErrorsSuppressed = true;
            webBrowserControl.AllowNavigation = false;
            webBrowserControl.ObjectForScripting = this;
            webBrowserControl.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserControl_DocumentCompleted);
        }

        void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Debug.WriteLine($">>> InternalPreviewControl: DocumentCompleted for URL={webBrowserControl.Url}");
            ScrollToElement(_activeSegId);
            webBrowserControl.Document.InvokeScript("setActiveStyle", new string[] { _activeSegId });
        }

        protected void FireWindowSelectionChanged()
        {
            Debug.WriteLine(">>> InternalPreviewControl: FireWindowSelectionChanged called");
            WindowSelectionChanged?.Invoke(null);
        }

        public void OpenTarget(string fileName)
        {
            Debug.WriteLine($">>> InternalPreviewControl: OpenTarget called with fileName={fileName}");
            if (InvokeRequired)
            {
                Invoke(new System.Action<string>(OpenTarget), fileName);
            }
            else
            {
                webBrowserControl.Navigate(fileName);
                webBrowserControl.Refresh();
            }
        }

        public void Close()
        {
            Debug.WriteLine(">>> InternalPreviewControl: Close called");
            // Trados handles temp file cleanup.
        }

        public SegmentReference GetSelectedSegment()
        {
            Debug.WriteLine($">>> InternalPreviewControl: GetSelectedSegment called, jumpsegmentID={_jumpsegmentID}");
            if (!string.IsNullOrEmpty(_jumpsegmentID))
            {
                return new SegmentReference(default(FileId), new ParagraphUnitId(_jumpparagraphID), new SegmentId(_jumpsegmentID));
            }
            return null;
        }

        public void SelectSegment(string paragraphUnitID, string segmentID)
        {
            Debug.WriteLine($">>> InternalPreviewControl: SelectSegment called with paragraphUnitID={paragraphUnitID}, segmentID={segmentID}");
            _jumpparagraphID = paragraphUnitID;
            _jumpsegmentID = segmentID;
            _segmentSelectedFromBrowser = true;
            FireWindowSelectionChanged();
        }

        private void ScrollToElement(string elemName)
        {
            Debug.WriteLine($">>> InternalPreviewControl: ScrollToElement called with elemName={elemName}");
            if (webBrowserControl.Document != null)
            {
                HtmlElementCollection elems = webBrowserControl.Document.All.GetElementsByName(elemName);
                if (elems != null && elems.Count > 0)
                {
                    elems[0].ScrollIntoView(true);
                }
            }
        }

        public void JumpToActiveElement()
        {
            Debug.WriteLine(">>> InternalPreviewControl: JumpToActiveElement called");
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(JumpToActiveElement));
            }
        }

        public void ScrollToSegment(SegmentReference segment)
        {
            Debug.WriteLine($">>> InternalPreviewControl: ScrollToSegment called with segmentID={segment?.SegmentId}");
            if (InvokeRequired)
            {
                Invoke(new System.Action<SegmentReference>(ScrollToSegment), segment);
            }
            else
            {
                if (!_segmentSelectedFromBrowser)
                {
                    ScrollToElement(segment.SegmentId.Id);

                    if (string.IsNullOrEmpty(_activeSegId))
                    {
                        _activeSegId = segment.SegmentId.Id;
                        webBrowserControl.Document.InvokeScript("setActiveStyle", new string[] { segment.SegmentId.Id });
                    }
                }

                if (_activeSegId != segment.SegmentId.Id)
                {
                    if (!string.IsNullOrEmpty(_activeSegId))
                    {
                        webBrowserControl.Document.InvokeScript("setNormalStyle", new string[] { _activeSegId });
                    }
                    webBrowserControl.Document.InvokeScript("setActiveStyle", new string[] { segment.SegmentId.Id });
                }

                _activeSegId = segment.SegmentId.Id;

                if (_segmentSelectedFromBrowser)
                {
                    _segmentSelectedFromBrowser = false;
                }
            }
        }

        public void UpdateIframe(string url)
        {
            Debug.WriteLine($">>> InternalPreviewControl: UpdateIframe called with url={url}");
            if (webBrowserControl.Document != null)
            {
                webBrowserControl.Document.InvokeScript("updateFrame", new string[] { url });
            }
        }
    }
}