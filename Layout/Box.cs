using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal abstract class Box
    {
        protected internal AreaTree areaTree = null;
        protected internal Area parent;

        public abstract void render( PdfRenderer renderer );
    }
}