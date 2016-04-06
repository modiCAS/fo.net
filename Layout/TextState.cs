namespace Fonet.Layout
{
    internal class TextState
    {
        protected bool linethrough;

        protected bool overlined;
        protected bool underlined;

        public bool getUnderlined()
        {
            return underlined;
        }

        public void setUnderlined( bool ul )
        {
            underlined = ul;
        }

        public bool getOverlined()
        {
            return overlined;
        }

        public void setOverlined( bool ol )
        {
            overlined = ol;
        }

        public bool getLineThrough()
        {
            return linethrough;
        }

        public void setLineThrough( bool lt )
        {
            linethrough = lt;
        }
    }
}