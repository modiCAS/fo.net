using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Marker : FObjMixed
    {
        private bool _isFirst;

        private bool _isLast;
        private readonly string _markerClassName;

        private Area _registryArea;

        public Marker( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:marker";

            _markerClassName =
                Properties.GetProperty( "marker-class-name" ).GetString();
            Ts = PropMgr.GetTextDecoration( parent );

            try
            {
                parent.AddMarker( _markerClassName );
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
            _registryArea = area;
            area.GetPage().RegisterMarker( this );
            return new Status( Status.Ok );
        }

        public Status LayoutMarker( Area area )
        {
            if ( Marker == MarkerStart )
                Marker = 0;

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];

                Status status;
                if ( ( status = fo.Layout( area ) ).IsIncomplete() )
                {
                    Marker = i;
                    return status;
                }
            }

            return new Status( Status.Ok );
        }

        public string GetMarkerClassName()
        {
            return _markerClassName;
        }

        public Area GetRegistryArea()
        {
            return _registryArea;
        }

        public void ReleaseRegistryArea()
        {
            _isFirst = _registryArea.IsFirst();
            _isLast = _registryArea.IsLast();
            _registryArea = null;
        }

        public void ResetMarkerArea()
        {
            if ( _registryArea != null )
            {
                Page page = _registryArea.GetPage();
                if ( page != null )
                    page.UnregisterMarker( this );
            }
        }

        public void ResetMarkerContent()
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