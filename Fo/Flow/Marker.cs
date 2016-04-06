using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Marker : FObjMixed
    {
        private bool isFirst;

        private bool isLast;
        private readonly string markerClassName;

        private Area registryArea;

        public Marker( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:marker";

            markerClassName =
                properties.GetProperty( "marker-class-name" ).GetString();
            ts = propMgr.getTextDecoration( parent );

            try
            {
                parent.AddMarker( markerClassName );
            }
            catch ( FonetException )
            {
            }
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            registryArea = area;
            area.getPage().registerMarker( this );
            return new Status( Status.OK );
        }

        public Status LayoutMarker( Area area )
        {
            if ( marker == MarkerStart )
                marker = 0;

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];

                Status status;
                if ( ( status = fo.Layout( area ) ).isIncomplete() )
                {
                    marker = i;
                    return status;
                }
            }

            return new Status( Status.OK );
        }

        public string GetMarkerClassName()
        {
            return markerClassName;
        }

        public Area GetRegistryArea()
        {
            return registryArea;
        }

        public void releaseRegistryArea()
        {
            isFirst = registryArea.isFirst();
            isLast = registryArea.isLast();
            registryArea = null;
        }

        public void resetMarker()
        {
            if ( registryArea != null )
            {
                Page page = registryArea.getPage();
                if ( page != null )
                    page.unregisterMarker( this );
            }
        }

        public void resetMarkerContent()
        {
            ResetMarker();
        }

        public override bool MayPrecedeMarker()
        {
            return true;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Marker( parent, propertyList );
            }
        }
    }
}