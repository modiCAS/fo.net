using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class WordArea : InlineArea
    {
        private readonly string text;

        public WordArea(
            FontState fontState, float red, float green,
            float blue, string text, int width )
            : base( fontState, width, red, green, blue )
        {
            this.text = text;
            contentRectangleWidth = width;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderWordArea( this );
        }

        public string getText()
        {
            return text;
        }
    }
}