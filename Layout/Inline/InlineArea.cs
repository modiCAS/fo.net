namespace Fonet.Layout.Inline
{
    internal abstract class InlineArea : Area
    {
        protected int height;
        protected bool lineThrough;
        protected bool overlined;
        protected string pageNumberId = null;
        private readonly float red;
        private readonly float green;
        private readonly float blue;
        protected bool underlined;
        private int verticalAlign;
        private int xOffset;
        private int yOffset;

        public InlineArea(
            FontState fontState, int width, float red,
            float green, float blue )
            : base( fontState )
        {
            contentRectangleWidth = width;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public float getBlue()
        {
            return blue;
        }

        public float getGreen()
        {
            return green;
        }

        public float getRed()
        {
            return red;
        }

        public override void SetHeight( int height )
        {
            this.height = height;
        }

        public override int GetHeight()
        {
            return height;
        }

        public virtual void setVerticalAlign( int align )
        {
            verticalAlign = align;
        }

        public virtual int getVerticalAlign()
        {
            return verticalAlign;
        }

        public void setYOffset( int yOffset )
        {
            this.yOffset = yOffset;
        }

        public int getYOffset()
        {
            return yOffset;
        }

        public void setXOffset( int xOffset )
        {
            this.xOffset = xOffset;
        }

        public virtual int getXOffset()
        {
            return xOffset;
        }

        public string getPageNumberID()
        {
            return pageNumberId;
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
    }
}