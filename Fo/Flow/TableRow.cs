using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableRow : FObj
    {
        private bool _areaAdded;

        private AreaContainer _areaContainer;

        private bool _bIgnoreKeepTogether;

        private int _breakAfter;

        private CellArray _cellArray;

        private ArrayList _columns;

        private string _id;

        private KeepValue _keepTogether;

        private KeepValue _keepWithNext;

        private KeepValue _keepWithPrevious;

        private int _largestCellHeight;

        private int _minHeight;

        private RowSpanMgr _rowSpanMgr;

        private bool _setup;

        public TableRow( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            if ( !( parent is AbstractTableBody ) )
            {
                throw new FonetException( "A table row must be child of fo:table-body,"
                    + " fo:table-header or fo:table-footer, not "
                    + parent.GetName() );
            }

            Name = "fo:table-row";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public void SetColumns( ArrayList columns )
        {
            this._columns = columns;
        }

        public KeepValue GetKeepWithPrevious()
        {
            return _keepWithPrevious;
        }

        public KeepValue GetKeepWithNext()
        {
            return _keepWithNext;
        }

        public KeepValue GetKeepTogether()
        {
            return _keepTogether;
        }

        public void DoSetup( Area area )
        {
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

            _breakAfter = Properties.GetProperty( "break-after" ).GetEnum();
            _keepTogether = GetKeepValue( "keep-together.within-column" );
            _keepWithNext = GetKeepValue( "keep-with-next.within-column" );
            _keepWithPrevious =
                GetKeepValue( "keep-with-previous.within-column" );

            _id = Properties.GetProperty( "id" ).GetString();
            _minHeight = Properties.GetProperty( "height" ).GetLength().MValue();
            _setup = true;
        }

        private KeepValue GetKeepValue( string sPropName )
        {
            Property p = Properties.GetProperty( sPropName );
            Number n = p.GetNumber();
            if ( n != null )
                return new KeepValue( KeepValue.KeepWithValue, n.IntValue() );
            switch ( p.GetEnum() )
            {
            case Constants.Always:
                return new KeepValue( KeepValue.KeepWithAlways, 0 );
            case Constants.Auto:
            default:
                return new KeepValue( KeepValue.KeepWithAuto, 0 );
            }
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                if ( !_setup )
                    DoSetup( area );

                if ( _cellArray == null )
                {
                    InitCellArray();
                    area.getIDReferences().CreateID( _id );
                }

                Marker = 0;
                int breakStatus = PropMgr.CheckBreakBefore( area );
                if ( breakStatus != Status.Ok )
                    return new Status( breakStatus );
            }

            if ( Marker == 0 )
                area.getIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.spaceLeft();

            _areaContainer =
                new AreaContainer( PropMgr.GetFontState( area.getFontInfo() ), 0, 0,
                    area.getContentWidth(), spaceLeft,
                    Position.Relative );
            _areaContainer.foCreator = this;
            _areaContainer.setPage( area.getPage() );
            _areaContainer.setParent( area );

            _areaContainer.setBackground( PropMgr.GetBackgroundProps() );
            _areaContainer.start();

            _areaContainer.setAbsoluteHeight( area.getAbsoluteHeight() );
            _areaContainer.setIDReferences( area.getIDReferences() );

            _largestCellHeight = _minHeight;

            var someCellDidNotLayoutCompletely = false;

            var offset = 0;
            var iColIndex = 0;

            foreach ( TableColumn tcol in _columns )
            {
                TableCell cell;
                ++iColIndex;
                int colWidth = tcol.GetColumnWidth();
                if ( _cellArray.GetCellType( iColIndex ) == CellArray.Cellstart )
                    cell = _cellArray.GetCell( iColIndex );
                else
                {
                    if ( _rowSpanMgr.IsInLastRow( iColIndex ) )
                    {
                        int h = _rowSpanMgr.GetRemainingHeight( iColIndex );
                        if ( h > _largestCellHeight )
                            _largestCellHeight = h;
                    }
                    offset += colWidth;
                    continue;
                }
                cell.SetStartOffset( offset );
                offset += colWidth;

                int rowSpan = cell.GetNumRowsSpanned();
                Status status;
                if ( ( status = cell.Layout( _areaContainer ) ).IsIncomplete() )
                {
                    if ( _keepTogether.GetKeepType() == KeepValue.KeepWithAlways
                        || status.GetCode() == Status.AreaFullNone
                        || rowSpan > 1 )
                    {
                        ResetMarker();
                        RemoveID( area.getIDReferences() );
                        return new Status( Status.AreaFullNone );
                    }
                    if ( status.GetCode() == Status.AreaFullSome )
                        someCellDidNotLayoutCompletely = true;
                }
                int hi = cell.GetHeight();
                if ( rowSpan > 1 )
                {
                    _rowSpanMgr.AddRowSpan( cell, iColIndex,
                        cell.GetNumColumnsSpanned(), hi,
                        rowSpan );
                }
                else if ( hi > _largestCellHeight )
                    _largestCellHeight = hi;
            }

            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                + _areaContainer.getMaxHeight() );

            for ( var iCol = 1; iCol <= _columns.Count; iCol++ )
            {
                if ( _cellArray.GetCellType( iCol ) == CellArray.Cellstart
                    && _rowSpanMgr.IsSpanned( iCol ) == false )
                    _cellArray.GetCell( iCol ).SetRowHeight( _largestCellHeight );
            }

            _rowSpanMgr.FinishRow( _largestCellHeight );

            area.addChild( _areaContainer );
            _areaContainer.SetHeight( _largestCellHeight );
            _areaAdded = true;
            _areaContainer.end();

            area.addDisplaySpace( _largestCellHeight
                + _areaContainer.getPaddingTop()
                + _areaContainer.getBorderTopWidth()
                + _areaContainer.getPaddingBottom()
                + _areaContainer.getBorderBottomWidth() );

            if ( someCellDidNotLayoutCompletely )
                return new Status( Status.AreaFullSome );
            if ( _rowSpanMgr.HasUnfinishedSpans() )
                return new Status( Status.KeepWithNext );
            if ( _breakAfter == GenericBreak.Enums.Page )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreak );
            }

            if ( _breakAfter == GenericBreak.Enums.OddPage )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreakOdd );
            }

            if ( _breakAfter == GenericBreak.Enums.EvenPage )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreakEven );
            }

            if ( _breakAfter == GenericBreak.Enums.Column )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForceColumnBreak );
            }
            if ( _keepWithNext.GetKeepType() != KeepValue.KeepWithAuto )
                return new Status( Status.KeepWithNext );
            return new Status( Status.Ok );
        }

        public int GetAreaHeight()
        {
            return _areaContainer.GetHeight();
        }

        public void RemoveLayout( Area area )
        {
            if ( _areaAdded )
                area.removeChild( _areaContainer );
            _areaAdded = false;
            ResetMarker();
            RemoveID( area.getIDReferences() );
        }

        public new void ResetMarker()
        {
            base.ResetMarker();
        }

        public void SetRowSpanMgr( RowSpanMgr rowSpanMgr )
        {
            this._rowSpanMgr = rowSpanMgr;
        }

        private void InitCellArray()
        {
            _cellArray = new CellArray( _rowSpanMgr, _columns.Count );
            var colNum = 1;
            foreach ( TableCell cell in Children )
            {
                colNum = _cellArray.GetNextFreeCell( colNum );
                int numCols = cell.GetNumColumnsSpanned();
                int numRows = cell.GetNumRowsSpanned();
                int cellColNum = cell.GetColumnNumber();

                if ( cellColNum == 0 )
                {
                    if ( colNum < 1 )
                        continue;
                    cellColNum = colNum;
                }
                else if ( cellColNum > _columns.Count )
                    continue;
                if ( cellColNum + numCols - 1 > _columns.Count )
                    numCols = _columns.Count - cellColNum + 1;
                if ( _cellArray.StoreCell( cell, cellColNum, numCols ) == false )
                {
                }
                if ( cellColNum > colNum )
                    colNum = cellColNum;
                else if ( cellColNum < colNum )
                    colNum = cellColNum;
                int cellWidth = GetCellWidth( cellColNum, numCols );
                cell.SetWidth( cellWidth );
                colNum += numCols;
            }
        }

        private int GetCellWidth( int startCol, int numCols )
        {
            var width = 0;
            for ( var count = 0; count < numCols; count++ )
                width += ( (TableColumn)_columns[ startCol + count - 1 ] ).GetColumnWidth();
            return width;
        }

        internal void SetIgnoreKeepTogether( bool bIgnoreKeepTogether )
        {
            this._bIgnoreKeepTogether = bIgnoreKeepTogether;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableRow( parent, propertyList );
            }
        }

        private class CellArray
        {
            public const byte Empty = 0;

            public const byte Cellstart = 1;

            public const byte Cellspan = 2;

            private readonly TableCell[] _cells;

            private readonly byte[] _states;

            internal CellArray( RowSpanMgr rsi, int numColumns )
            {
                _cells = new TableCell[ numColumns ];
                _states = new byte[ numColumns ];
                for ( var i = 0; i < numColumns; i++ )
                {
                    if ( rsi.IsSpanned( i + 1 ) )
                    {
                        _cells[ i ] = rsi.GetSpanningCell( i + 1 );
                        _states[ i ] = Cellspan;
                    }
                    else
                        _states[ i ] = Empty;
                }
            }

            internal int GetNextFreeCell( int colNum )
            {
                for ( int i = colNum - 1; i < _states.Length; i++ )
                {
                    if ( _states[ i ] == Empty )
                        return i + 1;
                }
                return -1;
            }

            internal int GetCellType( int colNum )
            {
                if ( colNum > 0 && colNum <= _cells.Length )
                    return _states[ colNum - 1 ];
                return -1;
            }

            internal TableCell GetCell( int colNum )
            {
                if ( colNum > 0 && colNum <= _cells.Length )
                    return _cells[ colNum - 1 ];
                return null;
            }

            internal bool StoreCell( TableCell cell, int colNum, int numCols )
            {
                var rslt = true;
                int index = colNum - 1;
                for ( var count = 0;
                    index < _cells.Length && count < numCols;
                    count++, index++ )
                {
                    if ( _cells[ index ] == null )
                    {
                        _cells[ index ] = cell;
                        _states[ index ] = count == 0 ? Cellstart : Cellspan;
                    }
                    else
                        rslt = false;
                }
                return rslt;
            }
        }
    }
}