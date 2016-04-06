using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class PageNumber : FObj
    {
        private float blue;
        private float green;

        private float red;
        private TextState ts;
        private int whiteSpaceCollapse;
        private int wrapOption;

        public PageNumber( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:page-number";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Page-number outside block area" );
                return new Status( Status.OK );
            }
            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginInlineProps mProps = propMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                ColorType c = properties.GetProperty( "color" ).GetColorType();
                red = c.Red;
                green = c.Green;
                blue = c.Blue;

                wrapOption = properties.GetProperty( "wrap-option" ).GetEnum();
                whiteSpaceCollapse =
                    properties.GetProperty( "white-space-collapse" ).GetEnum();
                ts = new TextState();
                marker = 0;

                string id = properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            string p = area.getPage().getFormattedNumber();
            marker = FOText.addText( (BlockArea)area,
                propMgr.GetFontState( area.getFontInfo() ),
                red, green, blue, wrapOption, null,
                whiteSpaceCollapse, p.ToCharArray(), 0,
                p.Length, ts, VerticalAlign.BASELINE );
            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new PageNumber( parent, propertyList );
            }
        }
    }
}