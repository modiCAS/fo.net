using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class Title : ToBeImplementedElement
    {
        protected Title( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:title";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            FontState fontState = PropMgr.GetFontState( area.getFontInfo() );
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();

            Property prop;
            prop = Properties.GetProperty( "baseline-shift" );
            if ( prop is LengthProperty )
            {
                Length bShift = prop.GetLength();
            }
            else if ( prop is EnumProperty )
            {
                int bShift = prop.GetEnum();
            }
            ColorType col = Properties.GetProperty( "color" ).GetColorType();
            Length lHeight = Properties.GetProperty( "line-height" ).GetLength();
            int lShiftAdj = Properties.GetProperty(
                "line-height-shift-adjustment" ).GetEnum();
            int vis = Properties.GetProperty( "visibility" ).GetEnum();
            Length zIndex = Properties.GetProperty( "z-index" ).GetLength();

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