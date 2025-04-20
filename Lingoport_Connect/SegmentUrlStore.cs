using System.Collections.Generic;

namespace Lingoport.LocalyzerConnect.Preview
{
    public static class SegmentUrlStore
    {
        public static Dictionary<string, string> SegmentIdToUrl = new Dictionary<string, string>();

        public static void Clear()
        {
            SegmentIdToUrl.Clear();
        }
    }
}