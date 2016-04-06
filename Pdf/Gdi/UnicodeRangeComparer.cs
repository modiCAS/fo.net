using System.Collections;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     Summary description for UnicodeRangeComparer.
    /// </summary>
    internal class UnicodeRangeComparer : IComparer
    {
        public int Compare( object x, object y )
        {
            var left = (UnicodeRange)x;
            var charToLocate = (char)y;

            // Two unicode ranges will never overlap
            if ( left.End < charToLocate )
                return -1;

            if ( left.Start > charToLocate )
                return 1;

            return 0;
        }
    }
}