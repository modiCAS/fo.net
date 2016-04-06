using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class ForeignObjectArea : InlineArea
    {
        protected int aheight;
        protected int align;
        protected int awidth;
        private bool chauto;
        protected int cheight;
        private bool cwauto;
        protected int cwidth;
        protected Area foreignObject;
        private bool hauto;
        private int overflow;
        protected int scaling;
        protected int valign;
        private bool wauto;
        protected int width;
        protected int xOffset;

        public ForeignObjectArea( FontState fontState, int width )
            : base( fontState, width, 0, 0, 0 )
        {
        }

        public override void render( PdfRenderer renderer )
        {
            if ( foreignObject != null )
                renderer.RenderForeignObjectArea( this );
        }

        public override int getContentWidth()
        {
            return getEffectiveWidth();
        }

        public override int GetHeight()
        {
            return getEffectiveHeight();
        }

        public override int getContentHeight()
        {
            return getEffectiveHeight();
        }

        public override int getXOffset()
        {
            return xOffset;
        }

        public void setStartIndent( int startIndent )
        {
            xOffset = startIndent;
        }

        public void setObject( Area fobject )
        {
            foreignObject = fobject;
        }

        public Area getObject()
        {
            return foreignObject;
        }

        public void setSizeAuto( bool wa, bool ha )
        {
            wauto = wa;
            hauto = ha;
        }

        public void setContentSizeAuto( bool wa, bool ha )
        {
            cwauto = wa;
            chauto = ha;
        }

        public bool isContentWidthAuto()
        {
            return cwauto;
        }

        public bool isContentHeightAuto()
        {
            return chauto;
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

        public void setOverflow( int o )
        {
            overflow = o;
        }

        public int getOverflow()
        {
            return overflow;
        }

        public override void SetHeight( int height )
        {
            this.height = height;
        }

        public void SetWidth( int width )
        {
            this.width = width;
        }

        public void setContentHeight( int cheight )
        {
            this.cheight = cheight;
        }

        public void SetContentWidth( int cwidth )
        {
            this.cwidth = cwidth;
        }

        public void setScaling( int scaling )
        {
            this.scaling = scaling;
        }

        public int scalingMethod()
        {
            return scaling;
        }

        public void setIntrinsicWidth( int w )
        {
            awidth = w;
        }

        public void setIntrinsicHeight( int h )
        {
            aheight = h;
        }

        public int getIntrinsicHeight()
        {
            return aheight;
        }

        public int getIntrinsicWidth()
        {
            return awidth;
        }

        public int getEffectiveHeight()
        {
            if ( hauto )
            {
                if ( chauto )
                    return aheight;
                return cheight;
            }
            return height;
        }

        public int getEffectiveWidth()
        {
            if ( wauto )
            {
                if ( cwauto )
                    return awidth;
                return cwidth;
            }
            return width;
        }
    }
}