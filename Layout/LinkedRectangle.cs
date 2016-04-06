using System.Drawing;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal class LinkedRectangle
    {
        protected InlineArea inlineArea;
        protected LineArea lineArea;
        protected Rectangle link;

        public LinkedRectangle( Rectangle link, LineArea lineArea,
            InlineArea inlineArea )
        {
            this.link = link;
            this.lineArea = lineArea;
            this.inlineArea = inlineArea;
        }

        public LinkedRectangle( LinkedRectangle lr )
        {
            link = lr.getRectangle();
            lineArea = lr.getLineArea();
            inlineArea = lr.getInlineArea();
        }

        public void setRectangle( Rectangle link )
        {
            this.link = link;
        }

        public Rectangle getRectangle()
        {
            return link;
        }

        public LineArea getLineArea()
        {
            return lineArea;
        }

        public void setLineArea( LineArea lineArea )
        {
            this.lineArea = lineArea;
        }

        public InlineArea getInlineArea()
        {
            return inlineArea;
        }

        public void setLineArea( InlineArea inlineArea )
        {
            this.inlineArea = inlineArea;
        }

        public void setX( int x )
        {
            link.X = x;
        }

        public void setY( int y )
        {
            link.Y = y;
        }

        public void SetWidth( int width )
        {
            link.Width = width;
        }

        public void SetHeight( int height )
        {
            link.Height = height;
        }

        public int getX()
        {
            return link.X;
        }

        public int getY()
        {
            return link.Y;
        }

        public int getWidth()
        {
            return link.Width;
        }

        public int GetHeight()
        {
            return link.Height;
        }
    }
}