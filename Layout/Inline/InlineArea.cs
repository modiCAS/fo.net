namespace Fonet.Layout.Inline
{
    internal abstract class InlineArea : Area
    {
        protected int Height;
        private bool _lineThrough;
        private bool _overlined;
        protected string PageNumberId = null;
        private readonly float _red;
        private readonly float _green;
        private readonly float _blue;
        private bool _underlined;
        private int _verticalAlign;
        private int _xOffset;
        private int _yOffset;

        protected InlineArea(
            FontState fontState, int width, float red,
            float green, float blue )
            : base( fontState )
        {
            ContentRectangleWidth = width;
            _red = red;
            _green = green;
            _blue = blue;
        }

        public float GetBlue()
        {
            return _blue;
        }

        public float GetGreen()
        {
            return _green;
        }

        public float GetRed()
        {
            return _red;
        }

        public override void SetHeight( int height )
        {
            Height = height;
        }

        public override int GetHeight()
        {
            return Height;
        }

        public virtual void SetVerticalAlign( int align )
        {
            _verticalAlign = align;
        }

        public virtual int GetVerticalAlign()
        {
            return _verticalAlign;
        }

        public void SetYOffset( int yOffset )
        {
            _yOffset = yOffset;
        }

        public int GetYOffset()
        {
            return _yOffset;
        }

        public void SetXOffset( int xOffset )
        {
            _xOffset = xOffset;
        }

        public virtual int GetXOffset()
        {
            return _xOffset;
        }

        public string GetPageNumberID()
        {
            return PageNumberId;
        }

        public void SetUnderlined( bool ul )
        {
            _underlined = ul;
        }

        public bool GetUnderlined()
        {
            return _underlined;
        }

        public void SetOverlined( bool ol )
        {
            _overlined = ol;
        }

        public bool GetOverlined()
        {
            return _overlined;
        }

        public void SetLineThrough( bool lt )
        {
            _lineThrough = lt;
        }

        public bool GetLineThrough()
        {
            return _lineThrough;
        }
    }
}