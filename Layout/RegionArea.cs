using Fonet.Fo.Properties;

namespace Fonet.Layout
{
    internal class RegionArea
    {
        protected BackgroundProps Background;
        protected int Height;
        protected int Width;
        protected int XPosition;
        protected int YPosition;

        public RegionArea( int xPosition, int yPosition, int width, int height )
        {
            this.XPosition = xPosition;
            this.YPosition = yPosition;
            this.Width = width;
            this.Height = height;
        }

        public AreaContainer MakeAreaContainer()
        {
            var area = new AreaContainer(
                null, XPosition, YPosition, Width, Height, Position.Absolute );
            area.SetBackground( GetBackground() );
            return area;
        }

        public BackgroundProps GetBackground()
        {
            return Background;
        }

        public void SetBackground( BackgroundProps bg )
        {
            Background = bg;
        }

        public int GetHeight()
        {
            return Height;
        }
    }
}