using Fonet.Layout;
using Fonet.Layout.Inline;
using Fonet.Render.Pdf;

namespace Fonet.Image
{
    internal class ImageArea : InlineArea
    {
        private int _align;
        private readonly FonetImage _image;
        private int _valign;
        private int _xOffset;

        public ImageArea( FontState fontState, FonetImage img, int allocationWidth,
            int width, int height, int startIndent, int endIndent,
            int align )
            : base( fontState, width, 0, 0, 0 )
        {
            CurrentHeight = height;
            ContentRectangleWidth = width;
            Height = height;
            _image = img;
            _align = align;
        }

        public override int GetXOffset()
        {
            return _xOffset;
        }

        public FonetImage GetImage()
        {
            return _image;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderImageArea( this );
        }

        public int GetImageHeight()
        {
            return CurrentHeight;
        }

        public void SetAlign( int align )
        {
            this._align = align;
        }

        public int GetAlign()
        {
            return _align;
        }

        public override void SetVerticalAlign( int align )
        {
            _valign = align;
        }

        public override int GetVerticalAlign()
        {
            return _valign;
        }

        public void SetStartIndent( int startIndent )
        {
            _xOffset = startIndent;
        }
    }
}