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
            for ( var i = 0; i < _spanInfo.Length; i++ )
            {
                if ( _spanInfo[ i ] != null )
                    return true;
            }
            return false;
        }

        public void FinishRow( int rowHeight )
        {
            for ( var i = 0; i < _spanInfo.Length; i++ )
            {
                if ( _spanInfo[ i ] != null && _spanInfo[ i ].finishRow( rowHeight ) )
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
                return _spanInfo[ colNum - 1 ].isInLastRow();
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

        public class SpanInfo
        {
            public TableCell Cell;
            public int CellHeight;
            public int RowsRemaining;
            public int TotalRowHeight;

            public SpanInfo( TableCell cell, int cellHeight, int rowsSpanned )
            {
                this.Cell = cell;
                this.CellHeight = cellHeight;
                TotalRowHeight = 0;
                RowsRemaining = rowsSpanned;
            }

            public int HeightRemaining()
            {
                int hl = CellHeight - TotalRowHeight;
                return hl > 0 ? hl : 0;
            }

            public bool isInLastRow()
            {
                return RowsRemaining == 1;
            }

            public bool finishRow( int rowHeight )
            {
                TotalRowHeight += rowHeight;
                if ( --RowsRemaining == 0 )
                {
                    if ( Cell != null )
                        Cell.SetRowHeight( TotalRowHeight );
                    return true;
                }
                return false;
            }
        }
    }
}