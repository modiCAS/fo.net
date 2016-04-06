using System.Collections;
using Fonet.Apps;
using Fonet.DataTypes;
using Fonet.Fo.Flow;
using Fonet.Fo.Pagination;
using Fonet.Layout;
using Fonet.Render.Pdf;

namespace Fonet
{
    /// <summary>
    ///     This class acts as a bridge between the XML:FO parser and the
    ///     formatting/rendering classes. It will queue PageSequences up until
    ///     all the IDs required by them are satisfied, at which time it will
    ///     render the pages.
    ///     StreamRenderer is created by Driver and called from FOTreeBuilder
    ///     when a PageSequence is created, and AreaTree when a Page is formatted.
    /// </summary>
    internal class StreamRenderer
    {
        private PageSequence _currentPageSequence;

        private ArrayList _currentPageSequenceMarkers;

        /// <summary>
        ///     The list of markers.
        /// </summary>
        private ArrayList _documentMarkers;

        /// <summary>
        ///     The FontInfo for this renderer.
        /// </summary>
        private readonly FontInfo _fontInfo = new FontInfo();

        /// <summary>
        ///     The current set of IDReferences, passed to the areatrees
        ///     and pages. This is used by the AreaTree as a single map of
        ///     all IDs.
        /// </summary>
        private readonly IDReferences _idReferences = new IDReferences();

        /// <summary>
        ///     The renderer being used.
        /// </summary>
        private readonly PdfRenderer _renderer;

        /// <summary>
        ///     The list of pages waiting to be renderered.
        /// </summary>
        private readonly ArrayList _renderQueue = new ArrayList();

        /// <summary>
        ///     The formatting results to be handed back to the caller.
        /// </summary>
        private readonly FormattingResults _results = new FormattingResults();

        public StreamRenderer( PdfRenderer renderer )
        {
            this._renderer = renderer;
        }

        /// <summary>
        ///     Keep track of the number of pages rendered.
        /// </summary>
        public int PageCount { get; private set; }

        public IDReferences GetIDReferences()
        {
            return _idReferences;
        }

        public FormattingResults GetResults()
        {
            return _results;
        }

        public void StartRenderer()
        {
            PageCount = 0;

            _renderer.SetupFontInfo( _fontInfo );
            _renderer.StartRenderer();
        }

        public void StopRenderer()
        {
            // Force the processing of any more queue elements, even if they 
            // are not resolved.
            ProcessQueue( true );
            _renderer.StopRenderer();
        }

        /// <summary>
        ///     Format the PageSequence. The PageSequence formats Pages and adds
        ///     them to the AreaTree, which subsequently calls the StreamRenderer
        ///     instance (this) again to render the page.  At this time the page
        ///     might be printed or it might be queued. A page might not be
        ///     renderable immediately if the IDReferences are not all valid. In
        ///     this case we defer the rendering until they are all valid.
        /// </summary>
        /// <param name="pageSequence"></param>
        public void Render( PageSequence pageSequence )
        {
            var a = new AreaTree( this );
            a.SetFontInfo( _fontInfo );

            pageSequence.Format( a );

            _results.HaveFormattedPageSequence( pageSequence );

            FonetDriver.ActiveDriver.FireFonetInfo(
                "Last page-sequence produced " + pageSequence.PageCount + " page(s)." );
        }

        public void QueuePage( Page page )
        {
            // Process markers
            PageSequence pageSequence = page.GetPageSequence();
            if ( pageSequence != _currentPageSequence )
            {
                _currentPageSequence = pageSequence;
                _currentPageSequenceMarkers = null;
            }
            ArrayList markers = page.GetMarkers();
            if ( markers != null )
            {
                if ( _documentMarkers == null )
                    _documentMarkers = new ArrayList();
                if ( _currentPageSequenceMarkers == null )
                    _currentPageSequenceMarkers = new ArrayList();
                for ( var i = 0; i < markers.Count; i++ )
                {
                    var marker = (Marker)markers[ i ];
                    marker.ReleaseRegistryArea();
                    _currentPageSequenceMarkers.Add( marker );
                    _documentMarkers.Add( marker );
                }
            }


            // Try to optimise on the common case that there are no pages pending 
            // and that all ID references are valid on the current pages. This 
            // short-cuts the pipeline and renders the area immediately.
            if ( _renderQueue.Count == 0 && _idReferences.IsEveryIdValid() )
                _renderer.Render( page );
            else
                AddToRenderQueue( page );

            PageCount++;
        }

        private void AddToRenderQueue( Page page )
        {
            var entry = new RenderQueueEntry( this, page );
            _renderQueue.Add( entry );

            // The just-added entry could (possibly) resolve the waiting entries, 
            // so we try to process the queue now to see.
            ProcessQueue( false );
        }

        /// <summary>
        ///     Try to process the queue from the first entry forward.  If an
        ///     entry can't be processed, then the queue can't move forward,
        ///     so return.
        /// </summary>
        /// <param name="force"></param>
        private void ProcessQueue( bool force )
        {
            while ( _renderQueue.Count > 0 )
            {
                var entry = (RenderQueueEntry)_renderQueue[ 0 ];
                if ( !force && !entry.IsResolved() )
                    break;

                _renderer.Render( entry.GetPage() );
                _renderQueue.RemoveAt( 0 );
            }
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetDocumentMarkers()
        {
            return _documentMarkers;
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public PageSequence GetCurrentPageSequence()
        {
            return _currentPageSequence;
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetCurrentPageSequenceMarkers()
        {
            return _currentPageSequenceMarkers;
        }

        /// <summary>
        ///     A RenderQueueEntry consists of the Page to be queued, plus a list
        ///     of outstanding ID references that need to be resolved before the
        ///     Page can be renderered.
        /// </summary>
        private class RenderQueueEntry
        {
            private readonly StreamRenderer _outer;

            /// <summary>
            ///     The Page that has outstanding ID references.
            /// </summary>
            private readonly Page _page;

            /// <summary>
            ///     A list of ID references (names).
            /// </summary>
            private readonly ArrayList _unresolvedIdReferences = new ArrayList();

            public RenderQueueEntry( StreamRenderer outer, Page page )
            {
                this._outer = outer;
                this._page = page;

                foreach ( object o in outer._idReferences.GetInvalidElements() )
                    _unresolvedIdReferences.Add( o );
            }

            public Page GetPage()
            {
                return _page;
            }

            /// <summary>
            ///     See if the outstanding references are resolved in the current
            ///     copy of IDReferences.
            /// </summary>
            /// <returns></returns>
            public bool IsResolved()
            {
                if ( _unresolvedIdReferences.Count == 0 || _outer._idReferences.IsEveryIdValid() )
                    return true;

                // See if any of the unresolved references are still unresolved.
                foreach ( string s in _unresolvedIdReferences )
                {
                    if ( !_outer._idReferences.DoesIDExist( s ) )
                        return false;
                }

                _unresolvedIdReferences.RemoveRange( 0, _unresolvedIdReferences.Count );
                return true;
            }
        }
    }
}