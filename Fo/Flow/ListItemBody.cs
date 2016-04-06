using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListItemBody : FObj
    {
        public ListItemBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:list-item-body";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                marker = 0;
                string id = properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FObj)children[ i ];

                Status status;
                if ( ( status = fo.Layout( area ) ).isIncomplete() )
                {
                    marker = i;
                    if ( i == 0 && status.getCode() == Status.AREA_FULL_NONE )
                        return new Status( Status.AREA_FULL_NONE );
                    return new Status( Status.AREA_FULL_SOME );
                }
            }
            return new Status( Status.OK );
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