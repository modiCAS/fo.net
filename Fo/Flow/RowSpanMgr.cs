using System.Linq;

namespace Fonet.Fo.Flow
{
    internal class RowSpanMgr
    {
        private bool _ignoreKeeps;

        private readonly SpanInfo[] _spanInfo;

        public RowSpanMgr( int numCols )
        {
            _spanInfo = new SpanInfo[ numCols ];
        }

        public void AddRowSpan( TableCell cell, int firstCol, int numCols,
            int cellHeight, int rowsSpanned )
        {
            _spanInfo[ firstCol - 1 ] = new SpanInfo( cell, cellHeight, rowsSpanned );
            for ( var i = 0; i < numCols - 1; i++ )
                _spanInfo[ firstCol + i ] = new SpanInfo( null, cellHeight, rowsSpanned );
        }

        public bool IsSpanned( int colNum )
        {
            return _spanInfo[ colNum - 1 ] != null;
        }

        public TableCell GetSpanningCell( int colNum )
        {
            if ( _spanInfo[ colNum - 1 ] != null )
                return _spanInfo[ colNum - 1 ].Cell;
            return null;
        }

        public bool HasUnfinishedSpans()
        {
            return _spanInfo.Any( t => t != null );
        }

        public void FinishRow( int rowHeight )
        {
            for ( var i = 0; i < _spanInfo.Length; i++ )
            {
                if ( _spanInfo[ i ] != null && _spanInfo[ i ].FinishRow( rowHeight ) )
                    _spanInfo[ i ] = null;
            }
        }

        public int GetRemainingHeight( int colNum )
        {
            if ( _spanInfo[ colNum - 1 ] != null )
                return _spanInfo[ colNum - 1 ].HeightRemaining();
            return 0;
        }

        public bool IsInLastRow( int colNum )
        {
            if ( _spanInfo[ colNum - 1 ] != null )
                return _spanInfo[ colNum - 1 ].IsInLastRow();
            return false;
        }

        public void SetIgnoreKeeps( bool ignoreKeeps )
        {
            this._ignoreKeeps = ignoreKeeps;
        }

        public bool IgnoreKeeps()
        {
            return _ignoreKeeps;
        }

        private sealed class SpanInfo
        {
            public readonly TableCell Cell;
            private readonly int _cellHeight;
            private int _rowsRemaining;
            private int _totalRowHeight;

            public SpanInfo( TableCell cell, int cellHeight, int rowsSpanned )
            {
                Cell = cell;
                _cellHeight = cellHeight;
                _totalRowHeight = 0;
                _rowsRemaining = rowsSpanned;
            }

            public int HeightRemaining()
            {
                int hl = _cellHeight - _totalRowHeight;
                return hl > 0 ? hl : 0;
            }

            public bool IsInLastRow()
            {
                return _rowsRemaining == 1;
            }

            public bool FinishRow( int rowHeight )
            {
                _totalRowHeight += rowHeight;
                if ( --_rowsRemaining != 0 ) return false;
                if ( Cell != null ) Cell.SetRowHeight( _totalRowHeight );
                return true;
            }
        }
    }
}