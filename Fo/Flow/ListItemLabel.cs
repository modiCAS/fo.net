using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListItemLabel : FObj
    {
        public ListItemLabel( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:list-item-label";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            int numChildren = children.Count;

            if ( numChildren != 1 )
                throw new FonetException( "list-item-label must have exactly one block in this version of FO.NET" );

            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            string id = properties.GetProperty( "id" ).GetString();
            area.getIDReferences().InitializeID( id, area );

            var block = (Block)children[ 0 ];

            Status status;
            status = block.Layout( area );
            area.addDisplaySpace( -block.GetAreaHeight() );
            return status;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListItemLabel( parent, propertyList );
            }
        }
    }
}