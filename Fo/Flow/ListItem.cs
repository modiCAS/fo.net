using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListItem : FObj
    {
        private int _align;
        private int _alignLast;
        private BlockArea _blockArea;
        private string _id;
        private int _lineHeight;
        private int _spaceAfter;
        private int _spaceBefore;

        public ListItem( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:list-item";
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
                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                _id = Properties.GetProperty( "id" ).GetString();

                area.getIDReferences().CreateID( _id );

                Marker = 0;
            }

            if ( area is BlockArea )
                area.end();

            if ( _spaceBefore != 0 )
                area.addDisplaySpace( _spaceBefore );

            _blockArea =
                new BlockArea( PropMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(), 0, 0,
                    0, _align, _alignLast, _lineHeight );
            _blockArea.setTableCellXOffset( area.getTableCellXOffset() );
            _blockArea.setGeneratedBy( this );
            AreasGenerated++;
            if ( AreasGenerated == 1 )
                _blockArea.isFirst( true );
            _blockArea.addLineagePair( this, AreasGenerated );

            _blockArea.setParent( area );
            _blockArea.setPage( area.getPage() );
            _blockArea.start();

            _blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            _blockArea.setIDReferences( area.getIDReferences() );

            int numChildren = Children.Count;
            if ( numChildren != 2 )
                throw new FonetException( "list-item must have exactly two children" );
            var label = (ListItemLabel)Children[ 0 ];
            var body = (ListItemBody)Children[ 1 ];

            Status status;

            if ( Marker == 0 )
            {
                area.getIDReferences().ConfigureID( _id, area );

                status = label.Layout( _blockArea );
                if ( status.IsIncomplete() )
                    return status;
            }

            status = body.Layout( _blockArea );
            if ( status.IsIncomplete() )
            {
                _blockArea.end();
                area.addChild( _blockArea );
                area.increaseHeight( _blockArea.GetHeight() );
                Marker = 1;
                return status;
            }

            _blockArea.end();
            area.addChild( _blockArea );
            area.increaseHeight( _blockArea.GetHeight() );

            if ( _spaceAfter != 0 )
                area.addDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.start();
            _blockArea.isLast( true );
            return new Status( Status.Ok );
        }

        public override int GetContentWidth()
        {
            if ( _blockArea != null )
                return _blockArea.getContentWidth();
            return 0;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListItem( parent, propertyList );
            }
        }
    }
}