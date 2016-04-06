using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class PropertyManager
    {
        private static readonly string msgColorFmt = "border-{0}-color";
        private static readonly string msgStyleFmt = "border-{0}-style";
        private static readonly string msgWidthFmt = "border-{0}-width";
        private static readonly string msgPaddingFmt = "padding-{0}";
        private BackgroundProps bgProps;
        private BorderAndPadding borderAndPadding;
        private FontState fontState;
        private HyphenationProps hyphProps;
        private readonly PropertyList properties;
        private string saBottom;

        private string saLeft;
        private string saRight;
        private string saTop;

        public PropertyManager( PropertyList pList )
        {
            properties = pList;
        }

        private void InitDirections()
        {
            saTop = properties.wmAbsToRel( PropertyList.TOP );
            saBottom = properties.wmAbsToRel( PropertyList.BOTTOM );
            saLeft = properties.wmAbsToRel( PropertyList.LEFT );
            saRight = properties.wmAbsToRel( PropertyList.RIGHT );
        }

        public FontState GetFontState( FontInfo fontInfo )
        {
            if ( fontState == null )
            {
                string fontFamily = properties.GetProperty( "font-family" ).GetString();
                string fontStyle = properties.GetProperty( "font-style" ).GetString();
                string fontWeight = properties.GetProperty( "font-weight" ).GetString();
                int fontSize = properties.GetProperty( "font-size" ).GetLength().MValue();
                int fontVariant = properties.GetProperty( "font-variant" ).GetEnum();
                fontState = new FontState( fontInfo, fontFamily, fontStyle,
                    fontWeight, fontSize, fontVariant );
            }
            return fontState;
        }


        public BorderAndPadding GetBorderAndPadding()
        {
            if ( borderAndPadding == null )
            {
                borderAndPadding = new BorderAndPadding();
                InitDirections();

                InitBorderInfo( BorderAndPadding.TOP, saTop );
                InitBorderInfo( BorderAndPadding.BOTTOM, saBottom );
                InitBorderInfo( BorderAndPadding.LEFT, saLeft );
                InitBorderInfo( BorderAndPadding.RIGHT, saRight );
            }
            return borderAndPadding;
        }

        private void InitBorderInfo( int whichSide, string saSide )
        {
            borderAndPadding.setPadding(
                whichSide, properties.GetProperty( string.Format( msgPaddingFmt, saSide ) ).GetCondLength() );
            int style = properties.GetProperty( string.Format( msgStyleFmt, saSide ) ).GetEnum();
            if ( style != Constants.NONE )
            {
                borderAndPadding.setBorder( whichSide, style,
                    properties.GetProperty( string.Format( msgWidthFmt, saSide ) ).GetCondLength(),
                    properties.GetProperty( string.Format( msgColorFmt, saSide ) ).GetColorType() );
            }
        }

        public HyphenationProps GetHyphenationProps()
        {
            if ( hyphProps == null )
            {
                hyphProps = new HyphenationProps();
                hyphProps.hyphenate = properties.GetProperty( "hyphenate" ).GetEnum();
                hyphProps.hyphenationChar =
                    properties.GetProperty( "hyphenation-character" ).GetCharacter();
                hyphProps.hyphenationPushCharacterCount =
                    properties.GetProperty( "hyphenation-push-character-count" ).GetNumber().IntValue();
                hyphProps.hyphenationRemainCharacterCount =
                    properties.GetProperty( "hyphenation-remain-character-count" ).GetNumber().IntValue();
                hyphProps.language = properties.GetProperty( "language" ).GetString();
                hyphProps.country = properties.GetProperty( "country" ).GetString();
            }
            return hyphProps;
        }

        public int CheckBreakBefore( Area area )
        {
            if ( !( area is ColumnArea ) )
            {
                switch ( properties.GetProperty( "break-before" ).GetEnum() )
                {
                case GenericBreak.Enums.PAGE:
                    return Status.FORCE_PAGE_BREAK;
                case GenericBreak.Enums.ODD_PAGE:
                    return Status.FORCE_PAGE_BREAK_ODD;
                case GenericBreak.Enums.EVEN_PAGE:
                    return Status.FORCE_PAGE_BREAK_EVEN;
                case GenericBreak.Enums.COLUMN:
                    return Status.FORCE_COLUMN_BREAK;
                default:
                    return Status.OK;
                }
            }
            var colArea = (ColumnArea)area;
            switch ( properties.GetProperty( "break-before" ).GetEnum() )
            {
            case GenericBreak.Enums.PAGE:
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1 )
                    return Status.OK;
                return Status.FORCE_PAGE_BREAK;
            case GenericBreak.Enums.ODD_PAGE:
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1
                    && colArea.getPage().getNumber() % 2 != 0 )
                    return Status.OK;
                return Status.FORCE_PAGE_BREAK_ODD;
            case GenericBreak.Enums.EVEN_PAGE:
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1
                    && colArea.getPage().getNumber() % 2 == 0 )
                    return Status.OK;
                return Status.FORCE_PAGE_BREAK_EVEN;
            case GenericBreak.Enums.COLUMN:
                if ( !area.hasChildren() )
                    return Status.OK;
                return Status.FORCE_COLUMN_BREAK;
            default:
                return Status.OK;
            }
        }

        public int CheckBreakAfter( Area area )
        {
            switch ( properties.GetProperty( "break-after" ).GetEnum() )
            {
            case GenericBreak.Enums.PAGE:
                return Status.FORCE_PAGE_BREAK;
            case GenericBreak.Enums.ODD_PAGE:
                return Status.FORCE_PAGE_BREAK_ODD;
            case GenericBreak.Enums.EVEN_PAGE:
                return Status.FORCE_PAGE_BREAK_EVEN;
            case GenericBreak.Enums.COLUMN:
                return Status.FORCE_COLUMN_BREAK;
            default:
                return Status.OK;
            }
        }

        public MarginProps GetMarginProps()
        {
            var props = new MarginProps();

            props.marginTop =
                properties.GetProperty( "margin-top" ).GetLength().MValue();
            props.marginBottom =
                properties.GetProperty( "margin-bottom" ).GetLength().MValue();
            props.marginLeft =
                properties.GetProperty( "margin-left" ).GetLength().MValue();
            props.marginRight =
                properties.GetProperty( "margin-right" ).GetLength().MValue();
            return props;
        }

        public BackgroundProps GetBackgroundProps()
        {
            if ( bgProps == null )
            {
                bgProps = new BackgroundProps();

                bgProps.backColor =
                    properties.GetProperty( "background-color" ).GetColorType();

                string src = properties.GetProperty( "background-image" ).GetString();
                if ( src == "none" )
                    bgProps.backImage = null;
                else if ( src == "inherit" )
                    bgProps.backImage = null;
                else
                {
                    try
                    {
                        bgProps.backImage = FonetImageFactory.Make( src );
                    }
                    catch ( FonetImageException imgex )
                    {
                        bgProps.backImage = null;
                        FonetDriver.ActiveDriver.FireFonetError( imgex.Message );
                    }
                }

                bgProps.backRepeat = properties.GetProperty( "background-repeat" ).GetEnum();
            }
            return bgProps;
        }

        public MarginInlineProps GetMarginInlineProps()
        {
            var props = new MarginInlineProps();
            return props;
        }

        public AccessibilityProps GetAccessibilityProps()
        {
            var props = new AccessibilityProps();
            string str;
            str = properties.GetProperty( "source-document" ).GetString();
            if ( !"none".Equals( str ) )
                props.sourceDoc = str;
            str = properties.GetProperty( "role" ).GetString();
            if ( !"none".Equals( str ) )
                props.role = str;
            return props;
        }

        public AuralProps GetAuralProps()
        {
            var props = new AuralProps();
            return props;
        }

        public RelativePositionProps GetRelativePositionProps()
        {
            var props = new RelativePositionProps();
            return props;
        }

        public AbsolutePositionProps GetAbsolutePositionProps()
        {
            var props = new AbsolutePositionProps();
            return props;
        }

        public TextState getTextDecoration( FObj parent )
        {
            TextState tsp = null;
            var found = false;

            do
            {
                string fname = parent.GetName();
                if ( fname.Equals( "fo:flow" ) || fname.Equals( "fo:static-content" ) )
                    found = true;
                else if ( fname.Equals( "fo:block" ) || fname.Equals( "fo:inline" ) )
                {
                    var fom = (FObjMixed)parent;
                    tsp = fom.getTextState();
                    found = true;
                }
                parent = parent.getParent();
            }
            while ( !found );

            var ts = new TextState();

            if ( tsp != null )
            {
                ts.setUnderlined( tsp.getUnderlined() );
                ts.setOverlined( tsp.getOverlined() );
                ts.setLineThrough( tsp.getLineThrough() );
            }

            int textDecoration = properties.GetProperty( "text-decoration" ).GetEnum();

            if ( textDecoration == TextDecoration.UNDERLINE )
                ts.setUnderlined( true );
            if ( textDecoration == TextDecoration.OVERLINE )
                ts.setOverlined( true );
            if ( textDecoration == TextDecoration.LINE_THROUGH )
                ts.setLineThrough( true );
            if ( textDecoration == TextDecoration.NO_UNDERLINE )
                ts.setUnderlined( false );
            if ( textDecoration == TextDecoration.NO_OVERLINE )
                ts.setOverlined( false );
            if ( textDecoration == TextDecoration.NO_LINE_THROUGH )
                ts.setLineThrough( false );

            return ts;
        }
    }
}