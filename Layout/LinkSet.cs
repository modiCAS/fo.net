using System.Collections;
using System.Drawing;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal class LinkSet
    {
        public const int Internal = 0;
        public const int External = 1;
        private readonly Area _area;
        private int _contentRectangleWidth;
        private readonly string _destination;
        protected int EndIndent = 0;
        private readonly int _linkType;
        private int _maxY;
        private ArrayList _rects = new ArrayList();
        protected int StartIndent = 0;
        private int _xoffset;
        private int _yoffset;

        public LinkSet( string destination, Area area, int linkType )
        {
            this._destination = destination;
            this._area = area;
            this._linkType = linkType;
        }

        public void AddRect( Rectangle r, LineArea lineArea, InlineArea inlineArea )
        {
            var linkedRectangle = new LinkedRectangle( r, lineArea, inlineArea );
            linkedRectangle.SetY( _yoffset );
            if ( _yoffset > _maxY )
                _maxY = _yoffset;
            _rects.Add( linkedRectangle );
        }

        public void SetYOffset( int y )
        {
            _yoffset = y;
        }

        public void SetXOffset( int x )
        {
            _xoffset = x;
        }

        public void SetContentRectangleWidth( int contentRectangleWidth )
        {
            this._contentRectangleWidth = contentRectangleWidth;
        }

        public void ApplyAreaContainerOffsets( AreaContainer ac, Area area )
        {
            int height = area.GetAbsoluteHeight();
            var ba = (BlockArea)area;
            foreach ( LinkedRectangle r in _rects )
            {
                r.SetX( r.GetX() + ac.GetXPosition() + area.GetTableCellXOffset() );
                r.SetY( ac.GetYPosition() - height + ( _maxY - r.GetY() ) - ba.GetHalfLeading() );
            }
        }

        public void MergeLinks()
        {
            int numRects = _rects.Count;
            if ( numRects == 1 )
                return;

            var curRect = new LinkedRectangle( (LinkedRectangle)_rects[ 0 ] );
            var nv = new ArrayList();

            for ( var ri = 1; ri < numRects; ri++ )
            {
                var r = (LinkedRectangle)_rects[ ri ];

                if ( r.GetLineArea() == curRect.GetLineArea() )
                    curRect.SetWidth( r.GetX() + r.GetWidth() - curRect.GetX() );
                else
                {
                    nv.Add( curRect );
                    curRect = new LinkedRectangle( r );
                }

                if ( ri == numRects - 1 )
                    nv.Add( curRect );
            }

            _rects = nv;
        }

        public void Align()
        {
            foreach ( LinkedRectangle r in _rects )
            {
                r.SetX( r.GetX() + r.GetLineArea().GetStartIndent()
                    + r.GetInlineArea().GetXOffset() );
            }
        }

        public string GetDest()
        {
            return _destination;
        }

        public ArrayList GetRects()
        {
            return _rects;
        }

        public int GetEndIndent()
        {
            return EndIndent;
        }

        public int GetStartIndent()
        {
            return StartIndent;
        }

        public Area GetArea()
        {
            return _area;
        }

        public int GetLinkType()
        {
            return _linkType;
        }
    }
}