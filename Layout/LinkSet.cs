using System.Collections;
using System.Drawing;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal class LinkSet
    {
        public const int INTERNAL = 0;
        public const int EXTERNAL = 1;
        private readonly Area area;
        private int contentRectangleWidth;
        private readonly string destination;
        protected int endIndent = 0;
        private readonly int linkType;
        private int maxY;
        private ArrayList rects = new ArrayList();
        protected int startIndent = 0;
        private int xoffset;
        private int yoffset;

        public LinkSet( string destination, Area area, int linkType )
        {
            this.destination = destination;
            this.area = area;
            this.linkType = linkType;
        }

        public void addRect( Rectangle r, LineArea lineArea, InlineArea inlineArea )
        {
            var linkedRectangle = new LinkedRectangle( r, lineArea, inlineArea );
            linkedRectangle.setY( yoffset );
            if ( yoffset > maxY )
                maxY = yoffset;
            rects.Add( linkedRectangle );
        }

        public void setYOffset( int y )
        {
            yoffset = y;
        }

        public void setXOffset( int x )
        {
            xoffset = x;
        }

        public void setContentRectangleWidth( int contentRectangleWidth )
        {
            this.contentRectangleWidth = contentRectangleWidth;
        }

        public void applyAreaContainerOffsets( AreaContainer ac, Area area )
        {
            int height = area.getAbsoluteHeight();
            var ba = (BlockArea)area;
            foreach ( LinkedRectangle r in rects )
            {
                r.setX( r.getX() + ac.getXPosition() + area.getTableCellXOffset() );
                r.setY( ac.GetYPosition() - height + ( maxY - r.getY() ) - ba.getHalfLeading() );
            }
        }

        public void mergeLinks()
        {
            int numRects = rects.Count;
            if ( numRects == 1 )
                return;

            var curRect = new LinkedRectangle( (LinkedRectangle)rects[ 0 ] );
            var nv = new ArrayList();

            for ( var ri = 1; ri < numRects; ri++ )
            {
                var r = (LinkedRectangle)rects[ ri ];

                if ( r.getLineArea() == curRect.getLineArea() )
                    curRect.SetWidth( r.getX() + r.getWidth() - curRect.getX() );
                else
                {
                    nv.Add( curRect );
                    curRect = new LinkedRectangle( r );
                }

                if ( ri == numRects - 1 )
                    nv.Add( curRect );
            }

            rects = nv;
        }

        public void align()
        {
            foreach ( LinkedRectangle r in rects )
            {
                r.setX( r.getX() + r.getLineArea().getStartIndent()
                    + r.getInlineArea().getXOffset() );
            }
        }

        public string getDest()
        {
            return destination;
        }

        public ArrayList getRects()
        {
            return rects;
        }

        public int getEndIndent()
        {
            return endIndent;
        }

        public int getStartIndent()
        {
            return startIndent;
        }

        public Area getArea()
        {
            return area;
        }

        public int getLinkType()
        {
            return linkType;
        }
    }
}