using Fonet.Fo.Properties;

namespace Fonet.Layout
{
    internal class BodyRegionArea : RegionArea
    {
        private int _columnCount;
        private int _columnGap;

        public BodyRegionArea(
            int xPosition, int yPosition, int width, int height )
            : base( xPosition, yPosition, width, height )
        {
        }

        public BodyAreaContainer MakeBodyAreaContainer()
        {
            var area = new BodyAreaContainer(
                null, XPosition, YPosition, Width,
                Height, Position.Absolute, _columnCount, _columnGap );
            area.SetBackground( GetBackground() );
            return area;
        }

        public void SetColumnCount( int columnCount )
        {
            this._columnCount = columnCount;
        }

        public int GetColumnCount()
        {
            return _columnCount;
        }

        public void SetColumnGap( int columnGap )
        {
            this._columnGap = columnGap;
        }

        public int GetColumnGap()
        {
            return _columnGap;
        }
    }
}