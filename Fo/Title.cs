using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class Title : ToBeImplementedElement
    {
        protected Title( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:title";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            FontState fontState = propMgr.GetFontState( area.getFontInfo() );
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();

            Property prop;
            prop = properties.GetProperty( "baseline-shift" );
            if ( prop is LengthProperty )
            {
                Length bShift = prop.GetLength();
            }
            else if ( prop is EnumProperty )
            {
                int bShift = prop.GetEnum();
            }
            ColorType col = properties.GetProperty( "color" ).GetColorType();
            Length lHeight = properties.GetProperty( "line-height" ).GetLength();
            int lShiftAdj = properties.GetProperty(
                "line-height-shift-adjustment" ).GetEnum();
            int vis = properties.GetProperty( "visibility" ).GetEnum();
            Length zIndex = properties.GetProperty( "z-index" ).GetLength();

            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Title( parent, propertyList );
            }
        }
    }
}