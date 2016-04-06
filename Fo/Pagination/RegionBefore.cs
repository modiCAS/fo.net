using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionBefore : Region
    {
        public const string REGION_CLASS = "before";

        private readonly int precedence;

        protected RegionBefore( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            precedence = properties.GetProperty( "precedence" ).GetEnum();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }


        public override RegionArea MakeRegionArea( int allocationRectangleXPosition,
            int allocationRectangleYPosition,
            int allocationRectangleWidth,
            int allocationRectangleHeight )
        {
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            int extent = properties.GetProperty( "extent" ).GetLength().MValue();

            var area = new RegionArea(
                allocationRectangleXPosition,
                allocationRectangleYPosition,
                allocationRectangleWidth,
                extent );
            area.setBackground( bProps );

            return area;
        }


        protected override string GetDefaultRegionName()
        {
            return "xsl-region-before";
        }

        protected override string GetElementName()
        {
            return "fo:region-before";
        }

        public override string GetRegionClass()
        {
            return REGION_CLASS;
        }

        public bool getPrecedence()
        {
            return precedence == Precedence.TRUE ? true : false;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RegionBefore( parent, propertyList );
            }
        }
    }
}