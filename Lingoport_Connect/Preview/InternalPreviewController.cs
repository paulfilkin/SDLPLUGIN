using System;
using System.Diagnostics;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Lingoport.LocalyzerConnect.Preview
{
    class InternalPreviewController : ISingleFilePreviewControl, INavigablePreview, IPreviewUpdatedViaRefresh, IDisposable
    {
        readonly InternalPreviewControl _control;
        private bool disposed = false;
        FileId _fileId;
        bool _isFileOpen = false;

        public InternalPreviewController()
        {
            Debug.WriteLine(">>> InternalPreviewController: Constructor called");
            _control = new InternalPreviewControl();
        }

        ~InternalPreviewController()
        {
            Dispose(false);
        }

        public System.Windows.Forms.Control Control => _control;

        public TempFileManager TargetFilePath
        {
            get => PreviewFile;
            set => PreviewFile = value;
        }

        void _Control_WindowSelectionChanged(IInteractivePreviewComponent component)
        {
            Debug.WriteLine(">>> InternalPreviewController: WindowSelectionChanged called");
            var marker = _control.GetSelectedSegment();
            SegmentReference selectedSegment = new SegmentReference(_fileId, marker.ParagraphUnitId, marker.SegmentId);
            OnSegmentSelected(this, new SegmentSelectedEventArgs(this, selectedSegment));
        }

        public void Refresh()
        {
            Debug.WriteLine(">>> InternalPreviewController: Refresh called");
            if (_isFileOpen)
            {
                _control.WindowSelectionChanged -= new PreviewControlHandler(_Control_WindowSelectionChanged);
                _control.Close();
            }

            if (PreviewFile != null)
            {
                _control.OpenTarget(PreviewFile.FilePath);
                _control.WindowSelectionChanged += new PreviewControlHandler(_Control_WindowSelectionChanged);
                _isFileOpen = true;
            }
            else
            {
                Debug.WriteLine(">>> InternalPreviewController: PreviewFile is null in Refresh");
            }
        }

        public TempFileManager PreviewFile { get; set; }

        public void NavigateToSegment(SegmentReference segment)
        {
            Debug.WriteLine($">>> InternalPreviewController: NavigateToSegment called with segmentID={segment?.SegmentId}");
            if (segment == null) return;
            _fileId = segment.FileId;
            _control.ScrollToSegment(segment);

            var segId = segment.SegmentId.Id;
            if (SegmentUrlStore.SegmentIdToUrl.TryGetValue(segId, out var url))
            {
                Debug.WriteLine($">>> InternalPreviewController: Updating iframe with url={url}");
                _control.UpdateIframe(url);
            }
            else
            {
                Debug.WriteLine($">>> InternalPreviewController: No URL found for segmentID={segId}");
            }
        }

        public System.Drawing.Color PreferredHighlightColor { get; set; }

        public event EventHandler<SegmentSelectedEventArgs> SegmentSelected;

        public virtual void OnSegmentSelected(object sender, SegmentSelectedEventArgs args)
        {
            Debug.WriteLine(">>> InternalPreviewController: OnSegmentSelected called");
            SegmentSelected?.Invoke(sender, args);
        }

        public void Dispose()
        {
            Debug.WriteLine(">>> InternalPreviewController: Dispose called");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (disposing && PreviewFile != null)
                    {
                        PreviewFile.Dispose();
                    }
                    disposed = true;
                }
                finally { }
            }
        }

        public void AfterFileRefresh()
        {
            Debug.WriteLine(">>> InternalPreviewController: AfterFileRefresh called");
            Refresh();
            ((InternalPreviewControl)Control).JumpToActiveElement();
        }

        public void BeforeFileRefresh()
        {
            Debug.WriteLine(">>> InternalPreviewController: BeforeFileRefresh called");
        }
    }
}