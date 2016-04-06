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

                area.GetIDReferences().CreateID( _id );

                Marker = 0;
            }

            if ( area is BlockArea )
                area.End();

            if ( _spaceBefore != 0 )
                area.AddDisplaySpace( _spaceBefore );

            _blockArea =
                new BlockArea( PropMgr.GetFontState( area.GetFontInfo() ),
                    area.GetAllocationWidth(), area.SpaceLeft(), 0, 0,
                    0, _align, _alignLast, _lineHeight );
            _blockArea.SetTableCellXOffset( area.GetTableCellXOffset() );
            _blockArea.SetGeneratedBy( this );
            AreasGenerated++;
            if ( AreasGenerated == 1 )
                _blockArea.IsFirst( true );
            _blockArea.AddLineagePair( this, AreasGenerated );

            _blockArea.SetParent( area );
            _blockArea.SetPage( area.GetPage() );
            _blockArea.Start();

            _blockArea.SetAbsoluteHeight( area.GetAbsoluteHeight() );
            _blockArea.SetIDReferences( area.GetIDReferences() );

            int numChildren = Children.Count;
            if ( numChildren != 2 )
                throw new FonetException( "list-item must have exactly two children" );
            var label = (ListItemLabel)Children[ 0 ];
            var body = (ListItemBody)Children[ 1 ];

            Status status;

            if ( Marker == 0 )
            {
                area.GetIDReferences().ConfigureID( _id, area );

                status = label.Layout( _blockArea );
                if ( status.IsIncomplete() )
                    return status;
            }

            status = body.Layout( _blockArea );
            if ( status.IsIncomplete() )
            {
                _blockArea.End();
                area.AddChild( _blockArea );
                area.IncreaseHeight( _blockArea.GetHeight() );
                Marker = 1;
                return status;
            }

            _blockArea.End();
            area.AddChild( _blockArea );
            area.IncreaseHeight( _blockArea.GetHeight() );

            if ( _spaceAfter != 0 )
                area.AddDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.Start();
            _blockArea.IsLast( true );
            return new Status( Status.Ok );
        }

        public override int GetContentWidth()
        {
            if ( _blockArea != null )
                return _blockArea.GetContentWidth();
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