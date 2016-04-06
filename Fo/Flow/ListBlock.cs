using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListBlock : FObj
    {
        private int _align;
        private int _alignLast;
        private int _endIndent;
        private int _lineHeight;
        private int _spaceAfter;
        private int _spaceBefore;
        private int _startIndent;

        public ListBlock( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:list-block";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerStart )
            {
                AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
                AuralProps mAurProps = PropMgr.GetAuralProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                MarginProps mProps = PropMgr.GetMarginProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                _align = Properties.GetProperty( "text-align" ).GetEnum();
                _alignLast = Properties.GetProperty( "text-align-last" ).GetEnum();
                _lineHeight =
                    Properties.GetProperty( "line-height" ).GetLength().MValue();
                _startIndent =
                    Properties.GetProperty( "start-indent" ).GetLength().MValue();
                _endIndent =
                    Properties.GetProperty( "end-indent" ).GetLength().MValue();
                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                Marker = 0;

                if ( area is BlockArea )
                    area.end();

                if ( _spaceBefore != 0 )
                    area.addDisplaySpace( _spaceBefore );

                if ( IsInTableCell )
                {
                    _startIndent += ForcedStartOffset;
                    _endIndent += area.getAllocationWidth() - ForcedWidth
                        - ForcedStartOffset;
                }

                string id = Properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            var blockArea =
                new BlockArea( PropMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(),
                    _startIndent, _endIndent, 0, _align, _alignLast,
                    _lineHeight );
            blockArea.setTableCellXOffset( area.getTableCellXOffset() );
            blockArea.setGeneratedBy( this );
            AreasGenerated++;
            if ( AreasGenerated == 1 )
                blockArea.isFirst( true );
            blockArea.addLineagePair( this, AreasGenerated );

            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.setBackground( PropMgr.GetBackgroundProps() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                if ( !( Children[ i ] is ListItem ) )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Children of list-blocks must be list-items" );
                    return new Status( Status.Ok );
                }
                var listItem = (ListItem)Children[ i ];
                Status status;
                if ( ( status = listItem.Layout( blockArea ) ).IsIncomplete() )
                {
                    if ( status.GetCode() == Status.AreaFullNone && i > 0 )
                        status = new Status( Status.AreaFullSome );
                    Marker = i;
                    blockArea.end();
                    area.addChild( blockArea );
                    area.increaseHeight( blockArea.GetHeight() );
                    return status;
                }
            }

            blockArea.end();
            area.addChild( blockArea );
            area.increaseHeight( blockArea.GetHeight() );

            if ( _spaceAfter != 0 )
                area.addDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.start();

            blockArea.isLast( true );
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListBlock( parent, propertyList );
            }
        }
    }
}