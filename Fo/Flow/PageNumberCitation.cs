using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class PageNumberCitation : FObj
    {
        private Area _area;
        private float _blue;
        private float _green;
        private string _id;
        private string _pageNumber;

        private float _red;
        private string _refId;
        private TextState _ts;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public PageNumberCitation( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:page-number-citation";
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
                return new Status( Status.Ok );
            }

            IDReferences idReferences = area.getIDReferences();
            this._area = area;
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

                _refId = Properties.GetProperty( "ref-id" ).GetString();

                if ( _refId.Equals( "" ) )
                    throw new FonetException( "page-number-citation must contain \"ref-id\"" );

                _id = Properties.GetProperty( "id" ).GetString();
                idReferences.CreateID( _id );
                _ts = new TextState();

                Marker = 0;
            }

            if ( Marker == 0 )
                idReferences.ConfigureID( _id, area );


            _pageNumber = idReferences.GetPageNumber( _refId );

            if ( _pageNumber != null )
            {
                Marker =
                    FoText.AddText( (BlockArea)area,
                        PropMgr.GetFontState( area.getFontInfo() ), _red,
                        _green, _blue, _wrapOption, null,
                        _whiteSpaceCollapse, _pageNumber.ToCharArray(),
                        0, _pageNumber.Length, _ts,
                        VerticalAlign.Baseline );
            }
            else
            {
                var blockArea = (BlockArea)area;
                LineArea la = blockArea.getCurrentLineArea();
                if ( la == null )
                    return new Status( Status.AreaFullNone );
                la.changeFont( PropMgr.GetFontState( area.getFontInfo() ) );
                la.changeColor( _red, _green, _blue );
                la.changeWrapOption( _wrapOption );
                la.changeWhiteSpaceCollapse( _whiteSpaceCollapse );
                la.addPageNumberCitation( _refId, null );
                Marker = -1;
            }

            if ( Marker == -1 )
                return new Status( Status.Ok );
            return new Status( Status.AreaFullNone );
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