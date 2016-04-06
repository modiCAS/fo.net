using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class DisplaySpace : Space
    {
        private readonly int _size;

        public DisplaySpace( int size )
        {
            _size = size;
        }

        public int GetSize()
        {
            return _size;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderDisplaySpace( this );
        }
    }
}