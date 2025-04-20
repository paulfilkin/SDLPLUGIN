using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Diagnostics;
using System.Collections.Generic;

namespace Lingoport.LocalyzerConnect.Preview
{
    class InternalPreviewWriter : AbstractNativeFileWriter, INativeContentCycleAware
    {
        private StreamWriter _preview;

        public InternalPreviewWriter()
        {
            Debug.WriteLine(">>> InternalPreviewWriter: Constructor called");
        }

        public void SetFileProperties(IFileProperties properties)
        {
            Debug.WriteLine(">>> InternalPreviewWriter: SetFileProperties called");
            // Not needed for preview
        }

        public void StartOfInput()
        {
            Debug.WriteLine(">>> InternalPreviewWriter: StartOfInput called");
            SegmentUrlStore.Clear();
            string previewFilePath = OutputProperties.OutputFilePath;
            _preview = new StreamWriter(previewFilePath);
            _preview.Write(GetHTMLTemplate("about:blank"));

            if (IsPxmlFile(previewFilePath))
            {
                TryExtractUrlsFromPxml(previewFilePath);
            }
            else if (IsSdlxliffFile(previewFilePath))
            {
                TryExtractUrlsFromSdlxliff(previewFilePath);
            }
        }

        public void EndOfInput()
        {
            Debug.WriteLine(">>> InternalPreviewWriter: EndOfInput called");
            _preview?.Write("</body></html>");
            _preview?.Close();
        }

        private bool IsPxmlFile(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() == ".pxml";
        }

        private bool IsSdlxliffFile(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() == ".sdlxliff";
        }

        private void TryExtractUrlsFromPxml(string filePath)
        {
            Debug.WriteLine($">>> InternalPreviewWriter: TryExtractUrlsFromPxml called for {filePath}");
            try
            {
                var doc = XDocument.Load(filePath);
                var strings = doc.Descendants("string");

                foreach (var element in strings)
                {
                    var translate = element.Attribute("translate")?.Value;
                    var segmentId = element.Attribute("segmentID")?.Value;
                    var url = element.Attribute("LRM_INCONTEXT")?.Value;

                    if (translate?.ToLowerInvariant() == "yes" &&
                        !string.IsNullOrEmpty(segmentId) &&
                        !string.IsNullOrEmpty(url) &&
                        url.StartsWith("http"))
                    {
                        SegmentUrlStore.SegmentIdToUrl[segmentId] = url.Trim();
                        Debug.WriteLine($">>> InternalPreviewWriter: Added URL for segmentID={segmentId}, url={url}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> InternalPreviewWriter: Error parsing PXML: {ex.Message}");
            }
        }

        private void TryExtractUrlsFromSdlxliff(string filePath)
        {
            Debug.WriteLine($">>> InternalPreviewWriter: TryExtractUrlsFromSdlxliff called for {filePath}");
            try
            {
                var doc = XDocument.Load(filePath);
                var ns = doc.Root.GetDefaultNamespace();
                var sdlNs = doc.Root.GetNamespaceOfPrefix("sdl");
                var commentDict = new Dictionary<string, string>();

                // Extract comments from cmt-def
                var cmtDefs = doc.Descendants(sdlNs + "cmt-def");
                foreach (var cmtDef in cmtDefs)
                {
                    var cmtId = cmtDef.Attribute("id")?.Value;
                    var comment = cmtDef.Descendants(sdlNs + "Comment").FirstOrDefault();
                    var url = comment?.Value;
                    if (!string.IsNullOrEmpty(cmtId) && url?.StartsWith("http") == true)
                    {
                        commentDict[cmtId] = url;
                        Debug.WriteLine($">>> InternalPreviewWriter: Found URL for cmtId={cmtId}, url={url}");
                    }
                }

                // Map trans-units to URLs
                var transUnits = doc.Descendants(ns + "trans-unit");
                foreach (var tu in transUnits)
                {
                    var segmentId = tu.Attribute("id")?.Value;
                    var cmtRef = tu.Descendants(sdlNs + "cmt").FirstOrDefault()?.Attribute("id")?.Value;
                    if (!string.IsNullOrEmpty(segmentId) && !string.IsNullOrEmpty(cmtRef) && commentDict.TryGetValue(cmtRef, out var url))
                    {
                        SegmentUrlStore.SegmentIdToUrl[segmentId] = url;
                        Debug.WriteLine($">>> InternalPreviewWriter: Added URL for segmentID={segmentId}, url={url}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> InternalPreviewWriter: Error parsing SDLXLIFF: {ex.Message}");
            }
        }

        private string GetHTMLTemplate(string url)
        {
            return $@"
            <!DOCTYPE html> <html> <head> <meta charset='utf-8'> <style> html, body {{ height: 100%; margin: 0; }} iframe {{ width: 100%; height: 100%; border: none; }} </style> <script> function updateFrame(url) {{ document.getElementById('previewFrame').src = url; }} function setActiveStyle(objDivID) {{ }} function setNormalStyle(objDivID) {{ }} </script> </head> <body> <iframe id='previewFrame' src='{url}'></iframe> ";
        }
    }
}