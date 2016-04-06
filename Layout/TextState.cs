namespace Fonet.Layout
{
    internal class TextState
    {
        protected bool Linethrough;

        protected bool Overlined;
        protected bool Underlined;

        public bool GetUnderlined()
        {
            return Underlined;
        }

        public void SetUnderlined( bool ul )
        {
            Underlined = ul;
        }

        public bool GetOverlined()
        {
            return Overlined;
        }

        public void SetOverlined( bool ol )
        {
            Overlined = ol;
        }

        public bool GetLineThrough()
        {
            return Linethrough;
        }

        public void SetLineThrough( bool lt )
        {
            Linethrough = lt;
        }
    }
}