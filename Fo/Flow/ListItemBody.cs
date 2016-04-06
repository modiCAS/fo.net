using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListItemBody : FObj
    {
        public ListItemBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:list-item-body";
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
                Marker = 0;
                string id = Properties.GetProperty( "id" ).GetString();
                area.GetIDReferences().InitializeID( id, area );
            }

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FObj)Children[ i ];

                Status status;
                if ( ( status = fo.Layout( area ) ).IsIncomplete() )
                {
                    Marker = i;
                    if ( i == 0 && status.GetCode() == Status.AreaFullNone )
                        return new Status( Status.AreaFullNone );
                    return new Status( Status.AreaFullSome );
                }
            }
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListItemBody( parent, propertyList );
            }
        }
    }
}