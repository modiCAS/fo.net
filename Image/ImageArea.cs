using Fonet.Layout;
using Fonet.Layout.Inline;
using Fonet.Render.Pdf;

namespace Fonet.Image
{
    internal class ImageArea : InlineArea
    {
        protected int align;
        protected FonetImage image;
        protected int valign;
        protected int xOffset;

        public ImageArea( FontState fontState, FonetImage img, int AllocationWidth,
            int width, int height, int startIndent, int endIndent,
            int align )
            : base( fontState, width, 0, 0, 0 )
        {
            currentHeight = height;
            contentRectangleWidth = width;
            this.height = height;
            image = img;
            this.align = align;
        }

        public override int getXOffset()
        {
            return xOffset;
        }

        public FonetImage getImage()
        {
            return image;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderImageArea( this );
        }

        public int getImageHeight()
        {
            return currentHeight;
        }

        public void setAlign( int align )
        {
            this.align = align;
        }

        public int getAlign()
        {
            return align;
        }

        public override void setVerticalAlign( int align )
        {
            valign = align;
        }

        public override int getVerticalAlign()
        {
            return valign;
        }

        public void setStartIndent( int startIndent )
        {
            xOffset = startIndent;
        }
    }
}