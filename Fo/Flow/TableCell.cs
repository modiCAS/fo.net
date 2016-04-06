using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableCell : FObj
    {
        private bool bDone;
        protected int beforeOffset;
        protected int borderHeight;
        protected bool bRelativeAlign;
        private bool bSepBorders = true;
        private AreaContainer cellArea;
        protected int height;
        private int iColNumber = -1;

        private string id;
        private int m_borderSeparation;
        protected int minCellHeight;
        private int numColumnsSpanned;
        private int numRowsSpanned;
        protected int startAdjust;
        protected int startOffset;
        protected int top;
        protected int verticalAlign;
        protected int width;
        protected int widthAdjust;

        public TableCell( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:table-cell";
            DoSetup();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public void SetStartOffset( int offset )
        {
            startOffset = offset;
        }

        public void SetWidth( int width )
        {
            this.width = width;
        }

        public int GetColumnNumber()
        {
            return iColNumber;
        }

        public int GetNumColumnsSpanned()
        {
            return numColumnsSpanned;
        }

        public int GetNumRowsSpanned()
        {
            return numRowsSpanned;
        }

        public void DoSetup()
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            iColNumber =
                properties.GetProperty( "column-number" ).GetNumber().IntValue();
            if ( iColNumber < 0 )
                iColNumber = 0;
            numColumnsSpanned =
                properties.GetProperty( "number-columns-spanned" ).GetNumber().IntValue();
            if ( numColumnsSpanned < 1 )
                numColumnsSpanned = 1;
            numRowsSpanned =
                properties.GetProperty( "number-rows-spanned" ).GetNumber().IntValue();
            if ( numRowsSpanned < 1 )
                numRowsSpanned = 1;

            id = properties.GetProperty( "id" ).GetString();

            bSepBorders = properties.GetProperty( "border-collapse" ).GetEnum()
                == BorderCollapse.SEPARATE;

            CalcBorders( propMgr.GetBorderAndPadding() );

            verticalAlign = properties.GetProperty( "display-align" ).GetEnum();
            if ( verticalAlign == DisplayAlign.AUTO )
            {
                bRelativeAlign = true;
                verticalAlign = properties.GetProperty( "relative-align" ).GetEnum();
            }
            else
                bRelativeAlign = false;

            minCellHeight =
                properties.GetProperty( "height" ).GetLength().MValue();
        }


        public override Status Layout( Area area )
        {
            int originalAbsoluteHeight = area.getAbsoluteHeight();
            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                area.getIDReferences().CreateID( id );

                marker = 0;
                bDone = false;
            }

            if ( marker == 0 )
                area.getIDReferences().ConfigureID( id, area );

            int spaceLeft = area.spaceLeft() - m_borderSeparation;
            cellArea =
                new AreaContainer( propMgr.GetFontState( area.getFontInfo() ),
                    startOffset + startAdjust, beforeOffset,
                    width - widthAdjust, spaceLeft,
                    Position.RELATIVE );

            cellArea.foCreator = this;
            cellArea.setPage( area.getPage() );
            cellArea.setParent( area );
            cellArea.setBorderAndPadding(
                (BorderAndPadding)propMgr.GetBorderAndPadding().Clone() );
            cellArea.setBackground( propMgr.GetBackgroundProps() );
            cellArea.start();

            cellArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            cellArea.setIDReferences( area.getIDReferences() );
            cellArea.setTableCellXOffset( startOffset + startAdjust );

            int numChildren = children.Count;
            for ( int i = marker; bDone == false && i < numChildren; i++ )
            {
                var fo = (FObj)children[ i ];
                fo.SetIsInTableCell();
                fo.ForceWidth( width );

                marker = i;

                Status status;
                if ( ( status = fo.Layout( cellArea ) ).isIncomplete() )
                {
                    if ( i == 0 && status.getCode() == Status.AREA_FULL_NONE )
                        return new Status( Status.AREA_FULL_NONE );
                    area.addChild( cellArea );
                    return new Status( Status.AREA_FULL_SOME );
                }

                area.setMaxHeight( area.getMaxHeight() - spaceLeft
                    + cellArea.getMaxHeight() );
            }
            bDone = true;
            cellArea.end();
            area.addChild( cellArea );

            if ( minCellHeight > cellArea.getContentHeight() )
                cellArea.SetHeight( minCellHeight );

            height = cellArea.GetHeight();
            top = cellArea.GetCurrentYPosition();

            return new Status( Status.OK );
        }

        public int GetHeight()
        {
            return cellArea.GetHeight() + m_borderSeparation - borderHeight;
        }

        public void SetRowHeight( int h )
        {
            int delta = h - GetHeight();
            if ( bRelativeAlign )
                cellArea.increaseHeight( delta );
            else if ( delta > 0 )
            {
                BorderAndPadding cellBP = cellArea.GetBorderAndPadding();
                switch ( verticalAlign )
                {
                case DisplayAlign.CENTER:
                    cellArea.shiftYPosition( delta / 2 );
                    cellBP.setPaddingLength( BorderAndPadding.TOP,
                        cellBP.getPaddingTop( false )
                            + delta / 2 );
                    cellBP.setPaddingLength( BorderAndPadding.BOTTOM,
                        cellBP.getPaddingBottom( false )
                            + delta - delta / 2 );
                    break;
                case DisplayAlign.AFTER:
                    cellBP.setPaddingLength( BorderAndPadding.TOP,
                        cellBP.getPaddingTop( false ) + delta );
                    cellArea.shiftYPosition( delta );
                    break;
                case DisplayAlign.BEFORE:
                    cellBP.setPaddingLength( BorderAndPadding.BOTTOM,
                        cellBP.getPaddingBottom( false )
                            + delta );
                    break;
                default:
                    break;
                }
            }
        }

        private void CalcBorders( BorderAndPadding bp )
        {
            if ( bSepBorders )
            {
                int iSep =
                    properties.GetProperty( "border-separation.inline-progression-direction" ).GetLength().MValue();
                startAdjust = iSep / 2 + bp.getBorderLeftWidth( false )
                    + bp.getPaddingLeft( false );
                widthAdjust = startAdjust + iSep - iSep / 2
                    + bp.getBorderRightWidth( false )
                    + bp.getPaddingRight( false );
                m_borderSeparation =
                    properties.GetProperty( "border-separation.block-progression-direction" ).GetLength().MValue();
                beforeOffset = m_borderSeparation / 2
                    + bp.getBorderTopWidth( false )
                    + bp.getPaddingTop( false );
            }
            else
            {
                int borderStart = bp.getBorderLeftWidth( false );
                int borderEnd = bp.getBorderRightWidth( false );
                int borderBefore = bp.getBorderTopWidth( false );
                int borderAfter = bp.getBorderBottomWidth( false );

                startAdjust = borderStart / 2 + bp.getPaddingLeft( false );

                widthAdjust = startAdjust + borderEnd / 2
                    + bp.getPaddingRight( false );
                beforeOffset = borderBefore / 2 + bp.getPaddingTop( false );
                borderHeight = ( borderBefore + borderAfter ) / 2;
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableCell( parent, propertyList );
            }
        }
    }
}