using System;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class RegionBody : Region
    {
        public const string RegionClass = "body";

        protected RegionBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
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
            MarginProps mProps = PropMgr.GetMarginProps();
            var body = new BodyRegionArea( allocationRectangleXPosition
                + mProps.marginLeft,
                allocationRectangleYPosition
                    - mProps.marginTop,
                allocationRectangleWidth
                    - mProps.marginLeft
                    - mProps.marginRight,
                allocationRectangleHeight
                    - mProps.marginTop
                    - mProps.marginBottom );

            body.setBackground( PropMgr.GetBackgroundProps() );

            int overflow = Properties.GetProperty( "overflow" ).GetEnum();
            string columnCountAsString =
                Properties.GetProperty( "column-count" ).GetString();
            var columnCount = 1;
            try
            {
                columnCount = int.Parse( columnCountAsString );
            }
            catch ( FormatException )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Bad value on region body 'column-count'" );
                columnCount = 1;
            }
            if ( columnCount > 1 && overflow == Overflow.Scroll )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Setting 'column-count' to 1 because 'overflow' is set to 'scroll'" );
                columnCount = 1;
            }
            body.setColumnCount( columnCount );

            int columnGap =
                Properties.GetProperty( "column-gap" ).GetLength().MValue();
            body.setColumnGap( columnGap );

            return body;
        }

        protected override string GetDefaultRegionName()
        {
            return "xsl-region-body";
        }

        protected override string GetElementName()
        {
            return "fo:region-body";
        }

        public override string GetRegionClass()
        {
            return RegionClass;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RegionBody( parent, propertyList );
            }
        }
    }
}