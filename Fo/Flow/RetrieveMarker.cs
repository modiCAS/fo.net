using System.Collections;
using Fonet.Fo.Pagination;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class RetrieveMarker : FObjMixed
    {
        private Marker bestMarker;

        private readonly int retrieveBoundary;
        private readonly string retrieveClassName;

        private readonly int retrievePosition;

        public RetrieveMarker( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:retrieve-marker";

            retrieveClassName =
                properties.GetProperty( "retrieve-class-name" ).GetString();
            retrievePosition =
                properties.GetProperty( "retrieve-position" ).GetEnum();
            retrieveBoundary =
                properties.GetProperty( "retrieve-boundary" ).GetEnum();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
            {
                marker = 0;
                Page containingPage = area.getPage();
                bestMarker = SearchPage( containingPage );

                if ( bestMarker != null )
                {
                    bestMarker.resetMarkerContent();
                    return bestMarker.LayoutMarker( area );
                }

                AreaTree areaTree = containingPage.getAreaTree();
                if ( retrieveBoundary == RetrieveBoundary.PAGE_SEQUENCE )
                {
                    PageSequence pageSequence = areaTree.GetCurrentPageSequence();
                    if ( pageSequence == containingPage.getPageSequence() )
                        return LayoutBestMarker( areaTree.GetCurrentPageSequenceMarkers(), area );
                }
                else if ( retrieveBoundary == RetrieveBoundary.DOCUMENT )
                    return LayoutBestMarker( areaTree.GetDocumentMarkers(), area );
                else if ( retrieveBoundary != RetrieveBoundary.PAGE )
                    throw new FonetException( "Illegal 'retrieve-boundary' value" );
            }
            else if ( bestMarker != null )
                return bestMarker.LayoutMarker( area );

            return new Status( Status.OK );
        }

        private Status LayoutBestMarker( ArrayList markers, Area area )
        {
            if ( markers != null )
            {
                for ( int i = markers.Count - 1; i >= 0; i-- )
                {
                    var currentMarker = (Marker)markers[ i ];
                    if ( currentMarker.GetMarkerClassName().Equals( retrieveClassName ) )
                    {
                        bestMarker = currentMarker;
                        bestMarker.resetMarkerContent();
                        return bestMarker.LayoutMarker( area );
                    }
                }
            }
            return new Status( Status.OK );
        }

        private Marker SearchPage( Page page )
        {
            ArrayList pageMarkers = page.getMarkers();
            if ( pageMarkers.Count == 0 )
                return null;

            if ( retrievePosition == RetrievePosition.FIC )
            {
                for ( var i = 0; i < pageMarkers.Count; i++ )
                {
                    var currentMarker = (Marker)pageMarkers[ i ];
                    if ( currentMarker.GetMarkerClassName().Equals( retrieveClassName ) )
                        return currentMarker;
                }
            }
            else if ( retrievePosition == RetrievePosition.FSWP )
            {
                for ( var c = 0; c < pageMarkers.Count; c++ )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( retrieveClassName ) )
                    {
                        if ( currentMarker.GetRegistryArea().isFirst() )
                            return currentMarker;
                    }
                }
            }
            else if ( retrievePosition == RetrievePosition.LSWP )
            {
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( retrieveClassName ) )
                    {
                        if ( currentMarker.GetRegistryArea().isFirst() )
                            return currentMarker;
                    }
                }
            }
            else if ( retrievePosition == RetrievePosition.LEWP )
            {
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( retrieveClassName ) )
                    {
                        if ( currentMarker.GetRegistryArea().isLast() )
                            return currentMarker;
                    }
                }
            }
            else
                throw new FonetException( "Illegal 'retrieve-position' value" );
            return null;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RetrieveMarker( parent, propertyList );
            }
        }
    }
}