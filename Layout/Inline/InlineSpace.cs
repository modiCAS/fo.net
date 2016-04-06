using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class InlineSpace : Space
    {
        private bool eatable;
        protected bool lineThrough;
        protected bool overlined;
        private bool resizeable = true;
        private int size;
        protected bool underlined;

        public InlineSpace( int amount )
        {
            size = amount;
        }

        public InlineSpace( int amount, bool resizeable )
        {
            this.resizeable = resizeable;
            size = amount;
        }

        public void setUnderlined( bool ul )
        {
            underlined = ul;
        }

        public bool getUnderlined()
        {
            return underlined;
        }

        public void setOverlined( bool ol )
        {
            overlined = ol;
        }

        public bool getOverlined()
        {
            return overlined;
        }

        public void setLineThrough( bool lt )
        {
            lineThrough = lt;
        }

        public bool getLineThrough()
        {
            return lineThrough;
        }

        public int getSize()
        {
            return size;
        }

        public void setSize( int amount )
        {
            size = amount;
        }

        public bool getResizeable()
        {
            return resizeable;
        }

        public void setResizeable( bool resizeable )
        {
            this.resizeable = resizeable;
        }

        public void setEatable( bool eatable )
        {
            this.eatable = eatable;
        }

        public bool isEatable()
        {
            return eatable;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderInlineSpace( this );
        }
    }
}