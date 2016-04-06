using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionBefore : Region
    {
        public const string RegionClass = "before";

        private readonly int _precedence;

        protected RegionBefore( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            _precedence = Properties.GetProperty( "precedence" ).GetEnum();
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
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            int extent = Properties.GetProperty( "extent" ).GetLength().MValue();

            var area = new RegionArea(
                allocationRectangleXPosition,
                allocationRectangleYPosition,
                allocationRectangleWidth,
                extent );
            area.SetBackground( bProps );

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
            return RegionClass;
        }

        public bool GetPrecedence()
        {
            return _precedence == Precedence.True;
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