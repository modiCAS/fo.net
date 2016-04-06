using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal abstract class Box
    {
        protected internal AreaTree AreaTree = null;
        protected internal Area Parent;

        public abstract void Render( PdfRenderer renderer );
    }
}