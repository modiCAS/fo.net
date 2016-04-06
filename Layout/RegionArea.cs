using Fonet.Fo.Properties;

namespace Fonet.Layout
{
    internal class RegionArea
    {
        protected BackgroundProps background;
        protected int height;
        protected int width;
        protected int xPosition;
        protected int yPosition;

        public RegionArea( int xPosition, int yPosition, int width, int height )
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.width = width;
            this.height = height;
        }

        public AreaContainer makeAreaContainer()
        {
            var area = new AreaContainer(
                null, xPosition, yPosition, width, height, Position.Absolute );
            area.setBackground( getBackground() );
            return area;
        }

        public BackgroundProps getBackground()
        {
            return background;
        }

        public void setBackground( BackgroundProps bg )
        {
            background = bg;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}