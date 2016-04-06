using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class BasicLink : Inline
    {
        public BasicLink( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:basic-link";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            string destination;
            int linkType;
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

            if ( !( destination =
                Properties.GetProperty( "internal-destination" ).GetString() ).Equals( "" ) )
                linkType = LinkSet.Internal;
            else if ( !( destination =
                Properties.GetProperty( "external-destination" ).GetString() ).Equals( "" ) )
                linkType = LinkSet.External;
            else
                throw new FonetException( "internal-destination or external-destination must be specified in basic-link" );

            if ( Marker == MarkerStart )
            {
                string id = Properties.GetProperty( "id" ).GetString();
                area.GetIDReferences().InitializeID( id, area );
                Marker = 0;
            }

            var ls = new LinkSet( destination, area, linkType );

            AreaContainer ac = area.GetNearestAncestorAreaContainer();
            while ( ac != null && ac.GetPosition() != Position.Absolute )
                ac = ac.GetNearestAncestorAreaContainer();
            if ( ac == null )
                ac = area.GetPage().GetBody().GetCurrentColumnArea();

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];
                fo.SetLinkSet( ls );

                Status status;
                if ( ( status = fo.Layout( area ) ).IsIncomplete() )
                {
                    Marker = i;
                    return status;
                }
            }

            ls.ApplyAreaContainerOffsets( ac, area );
            area.GetPage().AddLinkSet( ls );

            return new Status( Status.Ok );
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