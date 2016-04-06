using System.Collections;
using System.Linq;
using Fonet.Fo.Pagination;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal sealed class RetrieveMarker : FObjMixed
    {
        private Marker _bestMarker;

        private readonly int _retrieveBoundary;
        private readonly string _retrieveClassName;

        private readonly int _retrievePosition;

        private RetrieveMarker( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:retrieve-marker";

            _retrieveClassName =
                Properties.GetProperty( "retrieve-class-name" ).GetString();
            _retrievePosition =
                Properties.GetProperty( "retrieve-position" ).GetEnum();
            _retrieveBoundary =
                Properties.GetProperty( "retrieve-boundary" ).GetEnum();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerStart )
            {
                Marker = 0;
                Page containingPage = area.GetPage();
                _bestMarker = SearchPage( containingPage );

                if ( _bestMarker != null )
                {
                    _bestMarker.ResetMarkerContent();
                    return _bestMarker.LayoutMarker( area );
                }

                AreaTree areaTree = containingPage.GetAreaTree();
                if ( _retrieveBoundary == RetrieveBoundary.PageSequence )
                {
                    PageSequence pageSequence = areaTree.GetCurrentPageSequence();
                    if ( pageSequence == containingPage.GetPageSequence() )
                        return LayoutBestMarker( areaTree.GetCurrentPageSequenceMarkers(), area );
                }
                else if ( _retrieveBoundary == RetrieveBoundary.Document )
                    return LayoutBestMarker( areaTree.GetDocumentMarkers(), area );
                else if ( _retrieveBoundary != RetrieveBoundary.Page )
                    throw new FonetException( "Illegal 'retrieve-boundary' value" );
            }
            else if ( _bestMarker != null )
                return _bestMarker.LayoutMarker( area );

            return new Status( Status.Ok );
        }

        private Status LayoutBestMarker( ArrayList markers, Area area )
        {
            if ( markers != null )
            {
                for ( int i = markers.Count - 1; i >= 0; i-- )
                {
                    var currentMarker = (Marker)markers[ i ];
                    if ( currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) )
                    {
                        _bestMarker = currentMarker;
                        _bestMarker.ResetMarkerContent();
                        return _bestMarker.LayoutMarker( area );
                    }
                }
            }
            return new Status( Status.Ok );
        }

        private Marker SearchPage( Page page )
        {
            ArrayList pageMarkers = page.GetMarkers();
            if ( pageMarkers.Count == 0 )
                return null;

            switch ( _retrievePosition )
            {
            case RetrievePosition.Fic:
                return pageMarkers.Cast<Marker>()
                    .FirstOrDefault(
                        currentMarker => currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) );
            case RetrievePosition.Fswp:
                foreach ( Marker currentMarker in from Marker currentMarker in pageMarkers
                    where currentMarker.GetMarkerClassName().Equals( _retrieveClassName )
                    where currentMarker.GetRegistryArea().IsFirst()
                    select currentMarker )
                {
                    return currentMarker;
                }
                break;
            case RetrievePosition.Lswp:
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( !currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) ) continue;
                    if ( currentMarker.GetRegistryArea().IsFirst() )
                        return currentMarker;
                }
                break;
            case RetrievePosition.Lewp:
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( !currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) ) continue;
                    if ( currentMarker.GetRegistryArea().IsLast() )
                        return currentMarker;
                }
                break;
            default:
                throw new FonetException( "Illegal 'retrieve-position' value" );
            }
            return null;
        }

        private new sealed class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RetrieveMarker( parent, propertyList );
            }
        }
    }
}