using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class ForeignObjectArea : InlineArea
    {
        protected int Aheight;
        protected int Align;
        protected int Awidth;
        private bool _chauto;
        protected int Cheight;
        private bool _cwauto;
        protected int Cwidth;
        protected Area ForeignObject;
        private bool _hauto;
        private int _overflow;
        protected int Scaling;
        protected int Valign;
        private bool _wauto;
        protected int Width;
        protected int XOffset;

        public ForeignObjectArea( FontState fontState, int width )
            : base( fontState, width, 0, 0, 0 )
        {
        }

        public override void Render( PdfRenderer renderer )
        {
            if ( ForeignObject != null )
                renderer.RenderForeignObjectArea( this );
        }

        public override int GetContentWidth()
        {
            return GetEffectiveWidth();
        }

        public override int GetHeight()
        {
            return GetEffectiveHeight();
        }

        public override int GetContentHeight()
        {
            return GetEffectiveHeight();
        }

        public override int GetXOffset()
        {
            return XOffset;
        }

        public void SetStartIndent( int startIndent )
        {
            XOffset = startIndent;
        }

        public void SetObject( Area fobject )
        {
            ForeignObject = fobject;
        }

        public Area GetObject()
        {
            return ForeignObject;
        }

        public void SetSizeAuto( bool wa, bool ha )
        {
            _wauto = wa;
            _hauto = ha;
        }

        public void SetContentSizeAuto( bool wa, bool ha )
        {
            _cwauto = wa;
            _chauto = ha;
        }

        public bool IsContentWidthAuto()
        {
            return _cwauto;
        }

        public bool IsContentHeightAuto()
        {
            return _chauto;
        }

        public void SetAlign( int align )
        {
            this.Align = align;
        }

        public int GetAlign()
        {
            return Align;
        }

        public override void SetVerticalAlign( int align )
        {
            Valign = align;
        }

        public override int GetVerticalAlign()
        {
            return Valign;
        }

        public void SetOverflow( int o )
        {
            _overflow = o;
        }

        public int GetOverflow()
        {
            return _overflow;
        }

        public override void SetHeight( int height )
        {
            this.Height = height;
        }

        public void SetWidth( int width )
        {
            this.Width = width;
        }

        public void SetContentHeight( int cheight )
        {
            this.Cheight = cheight;
        }

        public void SetContentWidth( int cwidth )
        {
            this.Cwidth = cwidth;
        }

        public void SetScaling( int scaling )
        {
            this.Scaling = scaling;
        }

        public int ScalingMethod()
        {
            return Scaling;
        }

        public void SetIntrinsicWidth( int w )
        {
            Awidth = w;
        }

        public void SetIntrinsicHeight( int h )
        {
            Aheight = h;
        }

        public int GetIntrinsicHeight()
        {
            return Aheight;
        }

        public int GetIntrinsicWidth()
        {
            return Awidth;
        }

        public int GetEffectiveHeight()
        {
            if ( _hauto )
            {
                if ( _chauto )
                    return Aheight;
                return Cheight;
            }
            return Height;
        }

        public int GetEffectiveWidth()
        {
            if ( _wauto )
            {
                if ( _cwauto )
                    return Awidth;
                return Cwidth;
            }
            return Width;
        }
    }
}