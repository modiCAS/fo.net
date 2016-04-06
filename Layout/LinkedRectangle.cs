using System.Drawing;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal class LinkedRectangle
    {
        protected InlineArea InlineArea;
        protected LineArea LineArea;
        protected Rectangle Link;

        public LinkedRectangle( Rectangle link, LineArea lineArea,
            InlineArea inlineArea )
        {
            this.Link = link;
            this.LineArea = lineArea;
            this.InlineArea = inlineArea;
        }

        public LinkedRectangle( LinkedRectangle lr )
        {
            Link = lr.GetRectangle();
            LineArea = lr.GetLineArea();
            InlineArea = lr.GetInlineArea();
        }

        public void SetRectangle( Rectangle link )
        {
            this.Link = link;
        }

        public Rectangle GetRectangle()
        {
            return Link;
        }

        public LineArea GetLineArea()
        {
            return LineArea;
        }

        public void SetLineArea( LineArea lineArea )
        {
            this.LineArea = lineArea;
        }

        public InlineArea GetInlineArea()
        {
            return InlineArea;
        }

        public void SetLineArea( InlineArea inlineArea )
        {
            this.InlineArea = inlineArea;
        }

        public void SetX( int x )
        {
            Link.X = x;
        }

        public void SetY( int y )
        {
            Link.Y = y;
        }

        public void SetWidth( int width )
        {
            Link.Width = width;
        }

        public void SetHeight( int height )
        {
            Link.Height = height;
        }

        public int GetX()
        {
            return Link.X;
        }

        public int GetY()
        {
            return Link.Y;
        }

        public int GetWidth()
        {
            return Link.Width;
        }

        public int GetHeight()
        {
            return Link.Height;
        }
    }
}