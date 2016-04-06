using System.Collections;
using Fonet.Fo.Pagination;

namespace Fonet.Apps
{
    internal class FormattingResults
    {
        private int _pageCount;

        private ArrayList _pageSequences;

        internal int GetPageCount()
        {
            return _pageCount;
        }

        internal ArrayList GetPageSequences()
        {
            return _pageSequences;
        }

        internal void Reset()
        {
            _pageCount = 0;
            if ( _pageSequences != null )
                _pageSequences.Clear();
        }

        internal void HaveFormattedPageSequence( PageSequence pageSequence )
        {
            _pageCount += pageSequence.PageCount;
            if ( _pageSequences == null )
                _pageSequences = new ArrayList();

            _pageSequences.Add(
                new PageSequenceResults(
                    pageSequence.GetProperty( "id" ).GetString(),
                    pageSequence.PageCount ) );
        }
    }
}