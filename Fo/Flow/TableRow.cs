using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableRow : FObj
    {
        private bool areaAdded;

        private AreaContainer areaContainer;

        private bool bIgnoreKeepTogether;

        private int breakAfter;

        private CellArray cellArray;

        private ArrayList columns;

        private string id;

        private KeepValue keepTogether;

        private KeepValue keepWithNext;

        private KeepValue keepWithPrevious;

        private int largestCellHeight;

        private int minHeight;

        private RowSpanMgr rowSpanMgr;

        private bool setup;

        public TableRow( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            if ( !( parent is AbstractTableBody ) )
            {
                throw new FonetException( "A table row must be child of fo:table-body,"
                    + " fo:table-header or fo:table-footer, not "
                    + parent.GetName() );
            }

            name = "fo:table-row";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public void SetColumns( ArrayList columns )
        {
            this.columns = columns;
        }

        public KeepValue GetKeepWithPrevious()
        {
            return keepWithPrevious;
        }

        public KeepValue GetKeepWithNext()
        {
            return keepWithNext;
        }

        public KeepValue GetKeepTogether()
        {
            return keepTogether;
        }

        public void DoSetup( Area area )
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            breakAfter = properties.GetProperty( "break-after" ).GetEnum();
            keepTogether = getKeepValue( "keep-together.within-column" );
            keepWithNext = getKeepValue( "keep-with-next.within-column" );
            keepWithPrevious =
                getKeepValue( "keep-with-previous.within-column" );

            id = properties.GetProperty( "id" ).GetString();
            minHeight = properties.GetProperty( "height" ).GetLength().MValue();
            setup = true;
        }

        private KeepValue getKeepValue( string sPropName )
        {
            Property p = properties.GetProperty( sPropName );
            Number n = p.GetNumber();
            if ( n != null )
                return new KeepValue( KeepValue.KEEP_WITH_VALUE, n.IntValue() );
            switch ( p.GetEnum() )
            {
            case Constants.ALWAYS:
                return new KeepValue( KeepValue.KEEP_WITH_ALWAYS, 0 );
            case Constants.AUTO:
            default:
                return new KeepValue( KeepValue.KEEP_WITH_AUTO, 0 );
            }
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                if ( !setup )
                    DoSetup( area );

                if ( cellArray == null )
                {
                    InitCellArray();
                    area.getIDReferences().CreateID( id );
                }

                marker = 0;
                int breakStatus = propMgr.CheckBreakBefore( area );
                if ( breakStatus != Status.OK )
                    return new Status( breakStatus );
            }

            if ( marker == 0 )
                area.getIDReferences().ConfigureID( id, area );

            int spaceLeft = area.spaceLeft();

            areaContainer =
                new AreaContainer( propMgr.GetFontState( area.getFontInfo() ), 0, 0,
                    area.getContentWidth(), spaceLeft,
                    Position.RELATIVE );
            areaContainer.foCreator = this;
            areaContainer.setPage( area.getPage() );
            areaContainer.setParent( area );

            areaContainer.setBackground( propMgr.GetBackgroundProps() );
            areaContainer.start();

            areaContainer.setAbsoluteHeight( area.getAbsoluteHeight() );
            areaContainer.setIDReferences( area.getIDReferences() );

            largestCellHeight = minHeight;

            var someCellDidNotLayoutCompletely = false;

            var offset = 0;
            var iColIndex = 0;

            foreach ( TableColumn tcol in columns )
            {
                TableCell cell;
                ++iColIndex;
                int colWidth = tcol.GetColumnWidth();
                if ( cellArray.GetCellType( iColIndex ) == CellArray.CELLSTART )
                    cell = cellArray.GetCell( iColIndex );
                else
                {
                    if ( rowSpanMgr.IsInLastRow( iColIndex ) )
                    {
                        int h = rowSpanMgr.GetRemainingHeight( iColIndex );
                        if ( h > largestCellHeight )
                            largestCellHeight = h;
                    }
                    offset += colWidth;
                    continue;
                }
                cell.SetStartOffset( offset );
                offset += colWidth;

                int rowSpan = cell.GetNumRowsSpanned();
                Status status;
                if ( ( status = cell.Layout( areaContainer ) ).isIncomplete() )
                {
                    if ( keepTogether.GetKeepType() == KeepValue.KEEP_WITH_ALWAYS
                        || status.getCode() == Status.AREA_FULL_NONE
                        || rowSpan > 1 )
                    {
                        ResetMarker();
                        RemoveID( area.getIDReferences() );
                        return new Status( Status.AREA_FULL_NONE );
                    }
                    if ( status.getCode() == Status.AREA_FULL_SOME )
                        someCellDidNotLayoutCompletely = true;
                }
                int hi = cell.GetHeight();
                if ( rowSpan > 1 )
                {
                    rowSpanMgr.AddRowSpan( cell, iColIndex,
                        cell.GetNumColumnsSpanned(), hi,
                        rowSpan );
                }
                else if ( hi > largestCellHeight )
                    largestCellHeight = hi;
            }

            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                + areaContainer.getMaxHeight() );

            for ( var iCol = 1; iCol <= columns.Count; iCol++ )
            {
                if ( cellArray.GetCellType( iCol ) == CellArray.CELLSTART
                    && rowSpanMgr.IsSpanned( iCol ) == false )
                    cellArray.GetCell( iCol ).SetRowHeight( largestCellHeight );
            }

            rowSpanMgr.FinishRow( largestCellHeight );

            area.addChild( areaContainer );
            areaContainer.SetHeight( largestCellHeight );
            areaAdded = true;
            areaContainer.end();

            area.addDisplaySpace( largestCellHeight
                + areaContainer.getPaddingTop()
                + areaContainer.getBorderTopWidth()
                + areaContainer.getPaddingBottom()
                + areaContainer.getBorderBottomWidth() );

            if ( someCellDidNotLayoutCompletely )
                return new Status( Status.AREA_FULL_SOME );
            if ( rowSpanMgr.HasUnfinishedSpans() )
                return new Status( Status.KEEP_WITH_NEXT );
            if ( breakAfter == GenericBreak.Enums.PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK );
            }

            if ( breakAfter == GenericBreak.Enums.ODD_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_ODD );
            }

            if ( breakAfter == GenericBreak.Enums.EVEN_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_EVEN );
            }

            if ( breakAfter == GenericBreak.Enums.COLUMN )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_COLUMN_BREAK );
            }
            if ( keepWithNext.GetKeepType() != KeepValue.KEEP_WITH_AUTO )
                return new Status( Status.KEEP_WITH_NEXT );
            return new Status( Status.OK );
        }

        public int GetAreaHeight()
        {
            return areaContainer.GetHeight();
        }

        public void RemoveLayout( Area area )
        {
            if ( areaAdded )
                area.removeChild( areaContainer );
            areaAdded = false;
            ResetMarker();
            RemoveID( area.getIDReferences() );
        }

        public new void ResetMarker()
        {
            base.ResetMarker();
        }

        public void SetRowSpanMgr( RowSpanMgr rowSpanMgr )
        {
            this.rowSpanMgr = rowSpanMgr;
        }

        private void InitCellArray()
        {
            cellArray = new CellArray( rowSpanMgr, columns.Count );
            var colNum = 1;
            foreach ( TableCell cell in children )
            {
                colNum = cellArray.GetNextFreeCell( colNum );
                int numCols = cell.GetNumColumnsSpanned();
                int numRows = cell.GetNumRowsSpanned();
                int cellColNum = cell.GetColumnNumber();

                if ( cellColNum == 0 )
                {
                    if ( colNum < 1 )
                        continue;
                    cellColNum = colNum;
                }
                else if ( cellColNum > columns.Count )
                    continue;
                if ( cellColNum + numCols - 1 > columns.Count )
                    numCols = columns.Count - cellColNum + 1;
                if ( cellArray.StoreCell( cell, cellColNum, numCols ) == false )
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
                width += ( (TableColumn)columns[ startCol + count - 1 ] ).GetColumnWidth();
            return width;
        }

        internal void setIgnoreKeepTogether( bool bIgnoreKeepTogether )
        {
            this.bIgnoreKeepTogether = bIgnoreKeepTogether;
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
            public const byte EMPTY = 0;

            public const byte CELLSTART = 1;

            public const byte CELLSPAN = 2;

            private readonly TableCell[] cells;

            private readonly byte[] states;

            internal CellArray( RowSpanMgr rsi, int numColumns )
            {
                cells = new TableCell[ numColumns ];
                states = new byte[ numColumns ];
                for ( var i = 0; i < numColumns; i++ )
                {
                    if ( rsi.IsSpanned( i + 1 ) )
                    {
                        cells[ i ] = rsi.GetSpanningCell( i + 1 );
                        states[ i ] = CELLSPAN;
                    }
                    else
                        states[ i ] = EMPTY;
                }
            }

            internal int GetNextFreeCell( int colNum )
            {
                for ( int i = colNum - 1; i < states.Length; i++ )
                {
                    if ( states[ i ] == EMPTY )
                        return i + 1;
                }
                return -1;
            }

            internal int GetCellType( int colNum )
            {
                if ( colNum > 0 && colNum <= cells.Length )
                    return states[ colNum - 1 ];
                return -1;
            }

            internal TableCell GetCell( int colNum )
            {
                if ( colNum > 0 && colNum <= cells.Length )
                    return cells[ colNum - 1 ];
                return null;
            }

            internal bool StoreCell( TableCell cell, int colNum, int numCols )
            {
                var rslt = true;
                int index = colNum - 1;
                for ( var count = 0;
                    index < cells.Length && count < numCols;
                    count++, index++ )
                {
                    if ( cells[ index ] == null )
                    {
                        cells[ index ] = cell;
                        states[ index ] = count == 0 ? CELLSTART : CELLSPAN;
                    }
                    else
                        rslt = false;
                }
                return rslt;
            }
        }
    }
}