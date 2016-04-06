using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Flow;
using Fonet.Fo.Pagination;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class Page
    {
        private AreaContainer _after;
        private readonly AreaTree _areaTree;
        private AreaContainer _before;
        private BodyAreaContainer _body;
        private AreaContainer _end;
        private ArrayList _footnotes;
        protected string FormattedPageNumber;
        private readonly int _height;
        private readonly ArrayList _idList = new ArrayList();
        protected ArrayList LinkSets = new ArrayList();
        private readonly ArrayList _markers;
        protected int PageNumber;
        private PageSequence _pageSequence;
        private AreaContainer _start;
        private readonly int _width;

        internal Page( AreaTree areaTree, int height, int width )
        {
            this._areaTree = areaTree;
            this._height = height;
            this._width = width;
            _markers = new ArrayList();
        }

        public IDReferences GetIDReferences()
        {
            return _areaTree.GetIDReferences();
        }

        public void SetPageSequence( PageSequence pageSequence )
        {
            this._pageSequence = pageSequence;
        }

        public PageSequence GetPageSequence()
        {
            return _pageSequence;
        }

        public AreaTree GetAreaTree()
        {
            return _areaTree;
        }

        public void SetNumber( int number )
        {
            PageNumber = number;
        }

        public int GetNumber()
        {
            return PageNumber;
        }

        public void SetFormattedNumber( string number )
        {
            FormattedPageNumber = number;
        }

        public string GetFormattedNumber()
        {
            return FormattedPageNumber;
        }

        internal void AddAfter( AreaContainer area )
        {
            _after = area;
            area.SetPage( this );
        }

        internal void AddBefore( AreaContainer area )
        {
            _before = area;
            area.SetPage( this );
        }

        public void AddBody( BodyAreaContainer area )
        {
            _body = area;
            area.SetPage( this );
            area.GetMainReferenceArea().SetPage( this );
            area.GetBeforeFloatReferenceArea().SetPage( this );
            area.GetFootnoteReferenceArea().SetPage( this );
        }

        internal void AddEnd( AreaContainer area )
        {
            _end = area;
            area.SetPage( this );
        }

        internal void AddStart( AreaContainer area )
        {
            _start = area;
            area.SetPage( this );
        }

        public void Render( PdfRenderer renderer )
        {
            renderer.RenderPage( this );
        }

        public AreaContainer GetAfter()
        {
            return _after;
        }

        public AreaContainer GetBefore()
        {
            return _before;
        }

        public AreaContainer GetStart()
        {
            return _start;
        }

        public AreaContainer GetEnd()
        {
            return _end;
        }

        public BodyAreaContainer GetBody()
        {
            return _body;
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

        public FontInfo GetFontInfo()
        {
            return _areaTree.GetFontInfo();
        }

        public void AddLinkSet( LinkSet linkSet )
        {
            LinkSets.Add( linkSet );
        }

        public ArrayList GetLinkSets()
        {
            return LinkSets;
        }

        public bool HasLinks()
        {
            return LinkSets.Count != 0;
        }

        public void AddToIDList( string id )
        {
            _idList.Add( id );
        }

        public ArrayList GetIDList()
        {
            return _idList;
        }

        public ArrayList GetPendingFootnotes()
        {
            return _footnotes;
        }

        public void SetPendingFootnotes( ArrayList v )
        {
            _footnotes = v;
            if ( _footnotes != null )
            {
                foreach ( FootnoteBody fb in _footnotes )
                {
                    if ( !Footnote.LayoutFootnote( this, fb, null ) )
                    {
                        // footnotes are too large to fit on empty page.
                    }
                }
                _footnotes = null;
            }
        }

        public void AddPendingFootnote( FootnoteBody fb )
        {
            if ( _footnotes == null )
                _footnotes = new ArrayList();
            _footnotes.Add( fb );
        }

        public void UnregisterMarker( Marker marker )
        {
            _markers.Remove( marker );
        }

        public void RegisterMarker( Marker marker )
        {
            _markers.Add( marker );
        }

        public ArrayList GetMarkers()
        {
            return _markers;
        }
    }
}