using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionEnd : Region
    {
        public const string RegionClass = "end";

        protected RegionEnd( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }


        internal RegionArea MakeRegionArea( int allocationRectangleXPosition,
            int allocationRectangleYPosition,
            int allocationRectangleWidth,
            int allocationRectangleHeight,
            bool beforePrecedence,
            bool afterPrecedence, int beforeHeight,
            int afterHeight )
        {
            int extent = Properties.GetProperty( "extent" ).GetLength().MValue();

            int startY = allocationRectangleYPosition;
            int startH = allocationRectangleHeight;
            if ( beforePrecedence )
            {
                startY -= beforeHeight;
                startH -= beforeHeight;
            }

            if ( afterPrecedence )
                startH -= afterHeight;

            var area = new RegionArea(
                allocationRectangleXPosition + allocationRectangleWidth - extent,
                startY,
                extent,
                startH );
            area.setBackground( PropMgr.GetBackgroundProps() );

            return area;
        }

        public override RegionArea MakeRegionArea( int allocationRectangleXPosition,
            int allocationRectangleYPosition,
            int allocationRectangleWidth,
            int allocationRectangleHeight )
        {
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            int extent = Properties.GetProperty( "extent" ).GetLength().MValue();
            return MakeRegionArea( allocationRectangleXPosition,
                allocationRectangleYPosition,
                allocationRectangleWidth, extent, false, false,
                0, 0 );
        }

        protected override string GetDefaultRegionName()
        {
            return "xsl-region-end";
        }

        protected override string GetElementName()
        {
            return "fo:region-end";
        }

        public override string GetRegionClass()
        {
            return RegionClass;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RegionEnd( parent, propertyList );
            }
        }
    }
}