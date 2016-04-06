using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class PageNumber : FObj
    {
        private float _blue;
        private float _green;

        private float _red;
        private TextState _ts;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public PageNumber( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:page-number";
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
                return new Status( Status.Ok );
            }
            if ( Marker == MarkerStart )
            {
                AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
                AuralProps mAurProps = PropMgr.GetAuralProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                ColorType c = Properties.GetProperty( "color" ).GetColorType();
                _red = c.Red;
                _green = c.Green;
                _blue = c.Blue;

                _wrapOption = Properties.GetProperty( "wrap-option" ).GetEnum();
                _whiteSpaceCollapse =
                    Properties.GetProperty( "white-space-collapse" ).GetEnum();
                _ts = new TextState();
                Marker = 0;

                string id = Properties.GetProperty( "id" ).GetString();
                area.GetIDReferences().InitializeID( id, area );
            }

            string p = area.GetPage().GetFormattedNumber();
            Marker = FoText.AddText( (BlockArea)area,
                PropMgr.GetFontState( area.GetFontInfo() ),
                _red, _green, _blue, _wrapOption, null,
                _whiteSpaceCollapse, p.ToCharArray(), 0,
                p.Length, _ts, VerticalAlign.Baseline );
            return new Status( Status.Ok );
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