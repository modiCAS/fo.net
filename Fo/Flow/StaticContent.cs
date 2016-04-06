using Fonet.Fo.Pagination;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class StaticContent : Flow
    {
        protected StaticContent( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            ( (PageSequence)parent ).IsFlowSet = false;
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            return Layout( area, null );
        }

        public override Status Layout( Area area, Region region )
        {
            int numChildren = Children.Count;
            var regionClass = "none";
            if ( region != null )
                regionClass = region.GetRegionClass();
            else
            {
                if ( GetFlowName().Equals( "xsl-region-before" ) )
                    regionClass = RegionBefore.RegionClass;
                else if ( GetFlowName().Equals( "xsl-region-after" ) )
                    regionClass = RegionAfter.RegionClass;
                else if ( GetFlowName().Equals( "xsl-region-start" ) )
                    regionClass = RegionStart.RegionClass;
                else if ( GetFlowName().Equals( "xsl-region-end" ) )
                    regionClass = RegionEnd.RegionClass;
            }

            if ( area is AreaContainer )
                ( (AreaContainer)area ).SetAreaName( regionClass );

            area.SetAbsoluteHeight( 0 );

            SetContentWidth( area.GetContentWidth() );

            for ( var i = 0; i < numChildren; i++ )
            {
                var fo = (FObj)Children[ i ];

                Status status;
                if ( ( status = fo.Layout( area ) ).IsIncomplete() )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Some static content could not fit in the area." );
                    Marker = i;
                    if ( i != 0 && status.GetCode() == Status.AreaFullNone )
                        status = new Status( Status.AreaFullSome );
                    return status;
                }
            }
            ResetMarker();
            return new Status( Status.Ok );
        }

        protected override string GetElementName()
        {
            return "fo:static-content";
        }

        protected override void SetFlowName( string name )
        {
            if ( name == null || name.Equals( "" ) )
            {
                throw new FonetException( "A 'flow-name' is required for "
                    + GetElementName() + "." );
            }
            base.SetFlowName( name );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new StaticContent( parent, propertyList );
            }
        }
    }
}