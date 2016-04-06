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
            int originalAbsoluteHeight = area.GetAbsoluteHeight();
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                area.GetIDReferences().CreateID( _id );

                Marker = 0;
                _bDone = false;
            }

            if ( Marker == 0 )
                area.GetIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.SpaceLeft() - _mBorderSeparation;
            _cellArea =
                new AreaContainer( PropMgr.GetFontState( area.GetFontInfo() ),
                    StartOffset + StartAdjust, BeforeOffset,
                    Width - WidthAdjust, spaceLeft,
                    Position.Relative ) { FoCreator = this };

            _cellArea.SetPage( area.GetPage() );
            _cellArea.SetParent( area );
            _cellArea.SetBorderAndPadding(
                (BorderAndPadding)PropMgr.GetBorderAndPadding().Clone() );
            _cellArea.SetBackground( PropMgr.GetBackgroundProps() );
            _cellArea.Start();

            _cellArea.SetAbsoluteHeight( area.GetAbsoluteHeight() );
            _cellArea.SetIDReferences( area.GetIDReferences() );
            _cellArea.SetTableCellXOffset( StartOffset + StartAdjust );

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
                    area.AddChild( _cellArea );
                    return new Status( Status.AreaFullSome );
                }

                area.SetMaxHeight( area.GetMaxHeight() - spaceLeft
                    + _cellArea.GetMaxHeight() );
            }
            _bDone = true;
            _cellArea.End();
            area.AddChild( _cellArea );

            if ( MinCellHeight > _cellArea.GetContentHeight() )
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
                _cellArea.IncreaseHeight( delta );
            else if ( delta > 0 )
            {
                BorderAndPadding cellBp = _cellArea.GetBorderAndPadding();
                switch ( VerticalAlign )
                {
                case DisplayAlign.Center:
                    _cellArea.ShiftYPosition( delta / 2 );
                    cellBp.SetPaddingLength( BorderAndPadding.Top,
                        cellBp.GetPaddingTop( false )
                            + delta / 2 );
                    cellBp.SetPaddingLength( BorderAndPadding.Bottom,
                        cellBp.GetPaddingBottom( false )
                            + delta - delta / 2 );
                    break;
                case DisplayAlign.After:
                    cellBp.SetPaddingLength( BorderAndPadding.Top,
                        cellBp.GetPaddingTop( false ) + delta );
                    _cellArea.ShiftYPosition( delta );
                    break;
                case DisplayAlign.Before:
                    cellBp.SetPaddingLength( BorderAndPadding.Bottom,
                        cellBp.GetPaddingBottom( false )
                            + delta );
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
                StartAdjust = iSep / 2 + bp.GetBorderLeftWidth( false )
                    + bp.GetPaddingLeft( false );
                WidthAdjust = StartAdjust + iSep - iSep / 2
                    + bp.GetBorderRightWidth( false )
                    + bp.GetPaddingRight( false );
                _mBorderSeparation =
                    Properties.GetProperty( "border-separation.block-progression-direction" ).GetLength().MValue();
                BeforeOffset = _mBorderSeparation / 2
                    + bp.GetBorderTopWidth( false )
                    + bp.GetPaddingTop( false );
            }
            else
            {
                int borderStart = bp.GetBorderLeftWidth( false );
                int borderEnd = bp.GetBorderRightWidth( false );
                int borderBefore = bp.GetBorderTopWidth( false );
                int borderAfter = bp.GetBorderBottomWidth( false );

                StartAdjust = borderStart / 2 + bp.GetPaddingLeft( false );

                WidthAdjust = StartAdjust + borderEnd / 2
                    + bp.GetPaddingRight( false );
                BeforeOffset = borderBefore / 2 + bp.GetPaddingTop( false );
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