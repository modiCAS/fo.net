using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class BasicLink : Inline
    {
        public BasicLink( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:basic-link";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            string destination;
            int linkType;
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            if ( !( destination =
                properties.GetProperty( "internal-destination" ).GetString() ).Equals( "" ) )
                linkType = LinkSet.INTERNAL;
            else if ( !( destination =
                properties.GetProperty( "external-destination" ).GetString() ).Equals( "" ) )
                linkType = LinkSet.EXTERNAL;
            else
                throw new FonetException( "internal-destination or external-destination must be specified in basic-link" );

            if ( marker == MarkerStart )
            {
                string id = properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
                marker = 0;
            }

            var ls = new LinkSet( destination, area, linkType );

            AreaContainer ac = area.getNearestAncestorAreaContainer();
            while ( ac != null && ac.getPosition() != Position.ABSOLUTE )
                ac = ac.getNearestAncestorAreaContainer();
            if ( ac == null )
                ac = area.getPage().getBody().getCurrentColumnArea();

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                fo.SetLinkSet( ls );

                Status status;
                if ( ( status = fo.Layout( area ) ).isIncomplete() )
                {
                    marker = i;
                    return status;
                }
            }

            ls.applyAreaContainerOffsets( ac, area );
            area.getPage().addLinkSet( ls );

            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new BasicLink( parent, propertyList );
            }
        }
    }
}