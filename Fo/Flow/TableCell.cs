using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableCell : FObj
    {
        private bool _bDone;
        protected int BeforeOffset;
        protected int BorderHeight;
        protected bool BRelativeAlign;
        private bool _bSepBorders = true;
        private AreaContainer _cellArea;
        protected int Height;
        private int _iColNumber = -1;

        private string _id;
        private int _mBorderSeparation;
        protected int MinCellHeight;
        private int _numColumnsSpanned;
        private int _numRowsSpanned;
        protected int StartAdjust;
        protected int StartOffset;
        protected int Top;
        protected int VerticalAlign;
        protected int Width;
        protected int WidthAdjust;

        public TableCell( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table-cell";
            DoSetup();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public void SetStartOffset( int offset )
        {
            StartOffset = offset;
        }

        public void SetWidth( int width )
        {
            this.Width = width;
        }

        public int GetColumnNumber()
        {
            return _iColNumber;
        }

        public int GetNumColumnsSpanned()
        {
            return _numColumnsSpanned;
        }

        public int GetNumRowsSpanned()
        {
            return _numRowsSpanned;
        }

        public void DoSetup()
        {
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

            _iColNumber =
                Properties.GetProperty( "column-number" ).GetNumber().IntValue();
            if ( _iColNumber < 0 )
                _iColNumber = 0;
            _numColumnsSpanned =
                Properties.GetProperty( "number-columns-spanned" ).GetNumber().IntValue();
            if ( _numColumnsSpanned < 1 )
                _numColumnsSpanned = 1;
            _numRowsSpanned =
                Properties.GetProperty( "number-rows-spanned" ).GetNumber().IntValue();
            if ( _numRowsSpanned < 1 )
                _numRowsSpanned = 1;

            _id = Properties.GetProperty( "id" ).GetString();

            _bSepBorders = Properties.GetProperty( "border-collapse" ).GetEnum()
                == BorderCollapse.Separate;

            CalcBorders( PropMgr.GetBorderAndPadding() );

            VerticalAlign = Properties.GetProperty( "display-align" ).GetEnum();
            if ( VerticalAlign == DisplayAlign.Auto )
            {
                BRelativeAlign = true;
                VerticalAlign = Properties.GetProperty( "relative-align" ).GetEnum();
            }
            else
                BRelativeAlign = false;

            MinCellHeight =
                Properties.GetProperty( "height" ).GetLength().MValue();
        }


        public override Status Layout( Area area )
        {
            int originalAbsoluteHeight = area.getAbsoluteHeight();
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                area.getIDReferences().CreateID( _id );

                Marker = 0;
                _bDone = false;
            }

            if ( Marker == 0 )
                area.getIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.spaceLeft() - _mBorderSeparation;
            _cellArea =
                new AreaContainer( PropMgr.GetFontState( area.getFontInfo() ),
                    StartOffset + StartAdjust, BeforeOffset,
                    Width - WidthAdjust, spaceLeft,
                    Position.Relative );

            _cellArea.foCreator = this;
            _cellArea.setPage( area.getPage() );
            _cellArea.setParent( area );
            _cellArea.setBorderAndPadding(
                (BorderAndPadding)PropMgr.GetBorderAndPadding().Clone() );
            _cellArea.setBackground( PropMgr.GetBackgroundProps() );
            _cellArea.start();

            _cellArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            _cellArea.setIDReferences( area.getIDReferences() );
            _cellArea.setTableCellXOffset( StartOffset + StartAdjust );

            int numChildren = Children.Count;
            for ( int i = Marker; _bDone == false && i < numChildren; i++ )
            {
                var fo = (FObj)Children[ i ];
                fo.SetIsInTableCell();
                fo.ForceWidth( Width );

                Marker = i;

                Status status;
                if ( ( status = fo.Layout( _cellArea ) ).IsIncomplete() )
                {
                    if ( i == 0 && status.GetCode() == Status.AreaFullNone )
                        return new Status( Status.AreaFullNone );
                    area.addChild( _cellArea );
                    return new Status( Status.AreaFullSome );
                }

                area.setMaxHeight( area.getMaxHeight() - spaceLeft
                    + _cellArea.getMaxHeight() );
            }
            _bDone = true;
            _cellArea.end();
            area.addChild( _cellArea );

            if ( MinCellHeight > _cellArea.getContentHeight() )
                _cellArea.SetHeight( MinCellHeight );

            Height = _cellArea.GetHeight();
            Top = _cellArea.GetCurrentYPosition();

            return new Status( Status.Ok );
        }

        public int GetHeight()
        {
            return _cellArea.GetHeight() + _mBorderSeparation - BorderHeight;
        }

        public void SetRowHeight( int h )
        {
            int delta = h - GetHeight();
            if ( BRelativeAlign )
                _cellArea.increaseHeight( delta );
            else if ( delta > 0 )
            {
                BorderAndPadding cellBp = _cellArea.GetBorderAndPadding();
                switch ( VerticalAlign )
                {
                case DisplayAlign.Center:
                    _cellArea.shiftYPosition( delta / 2 );
                    cellBp.setPaddingLength( BorderAndPadding.TOP,
                        cellBp.getPaddingTop( false )
                            + delta / 2 );
                    cellBp.setPaddingLength( BorderAndPadding.BOTTOM,
                        cellBp.getPaddingBottom( false )
                            + delta - delta / 2 );
                    break;
                case DisplayAlign.After:
                    cellBp.setPaddingLength( BorderAndPadding.TOP,
                        cellBp.getPaddingTop( false ) + delta );
                    _cellArea.shiftYPosition( delta );
                    break;
                case DisplayAlign.Before:
                    cellBp.setPaddingLength( BorderAndPadding.BOTTOM,
                        cellBp.getPaddingBottom( false )
                            + delta );
                    break;
                default:
                    break;
                }
            }
        }

        private void CalcBorders( BorderAndPadding bp )
        {
            if ( _bSepBorders )
            {
                int iSep =
                    Properties.GetProperty( "border-separation.inline-progression-direction" ).GetLength().MValue();
                StartAdjust = iSep / 2 + bp.getBorderLeftWidth( false )
                    + bp.getPaddingLeft( false );
                WidthAdjust = StartAdjust + iSep - iSep / 2
                    + bp.getBorderRightWidth( false )
                    + bp.getPaddingRight( false );
                _mBorderSeparation =
                    Properties.GetProperty( "border-separation.block-progression-direction" ).GetLength().MValue();
                BeforeOffset = _mBorderSeparation / 2
                    + bp.getBorderTopWidth( false )
                    + bp.getPaddingTop( false );
            }
            else
            {
                int borderStart = bp.getBorderLeftWidth( false );
                int borderEnd = bp.getBorderRightWidth( false );
                int borderBefore = bp.getBorderTopWidth( false );
                int borderAfter = bp.getBorderBottomWidth( false );

                StartAdjust = borderStart / 2 + bp.getPaddingLeft( false );

                WidthAdjust = StartAdjust + borderEnd / 2
                    + bp.getPaddingRight( false );
                BeforeOffset = borderBefore / 2 + bp.getPaddingTop( false );
                BorderHeight = ( borderBefore + borderAfter ) / 2;
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