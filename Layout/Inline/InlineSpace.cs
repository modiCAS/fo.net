using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class InlineSpace : Space
    {
        private bool _eatable;
        protected bool LineThrough;
        protected bool Overlined;
        private bool _resizeable = true;
        private int _size;
        protected bool Underlined;

        public InlineSpace( int amount )
        {
            _size = amount;
        }

        public InlineSpace( int amount, bool resizeable )
        {
            this._resizeable = resizeable;
            _size = amount;
        }

        public void SetUnderlined( bool ul )
        {
            Underlined = ul;
        }

        public bool GetUnderlined()
        {
            return Underlined;
        }

        public void SetOverlined( bool ol )
        {
            Overlined = ol;
        }

        public bool GetOverlined()
        {
            return Overlined;
        }

        public void SetLineThrough( bool lt )
        {
            LineThrough = lt;
        }

        public bool GetLineThrough()
        {
            return LineThrough;
        }

        public int GetSize()
        {
            return _size;
        }

        public void SetSize( int amount )
        {
            _size = amount;
        }

        public bool GetResizeable()
        {
            return _resizeable;
        }

        public void SetResizeable( bool resizeable )
        {
            this._resizeable = resizeable;
        }

        public void SetEatable( bool eatable )
        {
            this._eatable = eatable;
        }

        public bool IsEatable()
        {
            return _eatable;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderInlineSpace( this );
        }
    }
}