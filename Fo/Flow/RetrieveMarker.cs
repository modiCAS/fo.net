using System.Collections;
using Fonet.Fo.Pagination;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class RetrieveMarker : FObjMixed
    {
        private Marker _bestMarker;

        private readonly int _retrieveBoundary;
        private readonly string _retrieveClassName;

        private readonly int _retrievePosition;

        public RetrieveMarker( FObj parent, PropertyList propertyList )
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
                Page containingPage = area.getPage();
                _bestMarker = SearchPage( containingPage );

                if ( _bestMarker != null )
                {
                    _bestMarker.ResetMarkerContent();
                    return _bestMarker.LayoutMarker( area );
                }

                AreaTree areaTree = containingPage.getAreaTree();
                if ( _retrieveBoundary == RetrieveBoundary.PageSequence )
                {
                    PageSequence pageSequence = areaTree.GetCurrentPageSequence();
                    if ( pageSequence == containingPage.getPageSequence() )
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
            ArrayList pageMarkers = page.getMarkers();
            if ( pageMarkers.Count == 0 )
                return null;

            if ( _retrievePosition == RetrievePosition.Fic )
            {
                for ( var i = 0; i < pageMarkers.Count; i++ )
                {
                    var currentMarker = (Marker)pageMarkers[ i ];
                    if ( currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) )
                        return currentMarker;
                }
            }
            else if ( _retrievePosition == RetrievePosition.Fswp )
            {
                for ( var c = 0; c < pageMarkers.Count; c++ )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) )
                    {
                        if ( currentMarker.GetRegistryArea().isFirst() )
                            return currentMarker;
                    }
                }
            }
            else if ( _retrievePosition == RetrievePosition.Lswp )
            {
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) )
                    {
                        if ( currentMarker.GetRegistryArea().isFirst() )
                            return currentMarker;
                    }
                }
            }
            else if ( _retrievePosition == RetrievePosition.Lewp )
            {
                for ( int c = pageMarkers.Count - 1; c >= 0; c-- )
                {
                    var currentMarker = (Marker)pageMarkers[ c ];
                    if ( currentMarker.GetMarkerClassName().Equals( _retrieveClassName ) )
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