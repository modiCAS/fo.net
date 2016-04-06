using System.Collections;
using Fonet.Fo.Pagination;

namespace Fonet.Apps
{
    internal class FormattingResults
    {
        private int pageCount;

        private ArrayList pageSequences;

        internal int GetPageCount()
        {
            return pageCount;
        }

        internal ArrayList GetPageSequences()
        {
            return pageSequences;
        }

        internal void Reset()
        {
            pageCount = 0;
            if ( pageSequences != null )
                pageSequences.Clear();
        }

        internal void HaveFormattedPageSequence( PageSequence pageSequence )
        {
            pageCount += pageSequence.PageCount;
            if ( pageSequences == null )
                pageSequences = new ArrayList();

            pageSequences.Add(
                new PageSequenceResults(
                    pageSequence.GetProperty( "id" ).GetString(),
                    pageSequence.PageCount ) );
        }
    }
}