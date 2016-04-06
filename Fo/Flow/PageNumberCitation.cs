using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class PageNumberCitation : FObj
    {
        private Area area;
        private float blue;
        private float green;
        private string id;
        private string pageNumber;

        private float red;
        private string refId;
        private TextState ts;
        private int whiteSpaceCollapse;
        private int wrapOption;

        public PageNumberCitation( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:page-number-citation";
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
                    "Page-number-citation outside block area" );
                return new Status( Status.OK );
            }

            IDReferences idReferences = area.getIDReferences();
            this.area = area;
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

                refId = properties.GetProperty( "ref-id" ).GetString();

                if ( refId.Equals( "" ) )
                    throw new FonetException( "page-number-citation must contain \"ref-id\"" );

                id = properties.GetProperty( "id" ).GetString();
                idReferences.CreateID( id );
                ts = new TextState();

                marker = 0;
            }

            if ( marker == 0 )
                idReferences.ConfigureID( id, area );


            pageNumber = idReferences.getPageNumber( refId );

            if ( pageNumber != null )
            {
                marker =
                    FOText.addText( (BlockArea)area,
                        propMgr.GetFontState( area.getFontInfo() ), red,
                        green, blue, wrapOption, null,
                        whiteSpaceCollapse, pageNumber.ToCharArray(),
                        0, pageNumber.Length, ts,
                        VerticalAlign.BASELINE );
            }
            else
            {
                var blockArea = (BlockArea)area;
                LineArea la = blockArea.getCurrentLineArea();
                if ( la == null )
                    return new Status( Status.AREA_FULL_NONE );
                la.changeFont( propMgr.GetFontState( area.getFontInfo() ) );
                la.changeColor( red, green, blue );
                la.changeWrapOption( wrapOption );
                la.changeWhiteSpaceCollapse( whiteSpaceCollapse );
                la.addPageNumberCitation( refId, null );
                marker = -1;
            }

            if ( marker == -1 )
                return new Status( Status.OK );
            return new Status( Status.AREA_FULL_NONE );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new PageNumberCitation( parent, propertyList );
            }
        }
    }
}