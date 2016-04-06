using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FOText : FONode
    {
        private float blue;
        protected char[] ca;

        private FontState fs;
        private float green;
        protected int length;
        protected bool lineThrough;
        protected bool overlined;
        private float red;
        protected int start;

        private TextState ts;

        protected bool underlined;
        private int verticalAlign;
        private int whiteSpaceCollapse;
        private int wrapOption;

        public FOText( char[] chars, int s, int e, FObj parent )
            : base( parent )
        {
            start = 0;
            ca = new char[ e - s ];
            for ( int i = s; i < e; i++ )
                ca[ i - s ] = chars[ i ];
            length = e - s;
        }

        public void setUnderlined( bool ul )
        {
            underlined = ul;
        }

        public void setOverlined( bool ol )
        {
            overlined = ol;
        }

        public void setLineThrough( bool lt )
        {
            lineThrough = lt;
        }

        public bool willCreateArea()
        {
            whiteSpaceCollapse =
                parent.properties.GetProperty( "white-space-collapse" ).GetEnum();
            if ( whiteSpaceCollapse == GenericBoolean.Enums.FALSE
                && length > 0 )
                return true;

            for ( int i = start; i < start + length; i++ )
            {
                char ch = ca[ i ];
                if ( !( ch == ' ' || ch == '\n' || ch == '\r'
                    || ch == '\t' ) )
                    return true;
            }
            return false;
        }

        public override bool MayPrecedeMarker()
        {
            for ( var i = 0; i < ca.Length; i++ )
            {
                char ch = ca[ i ];
                if ( ch != ' ' || ch != '\n' || ch != '\r' || ch != '\t' )
                    return true;
            }
            return false;
        }

        public override Status Layout( Area area )
        {
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Text outside block area" + new string( ca, start, length ) );
                return new Status( Status.OK );
            }
            if ( marker == MarkerStart )
            {
                string fontFamily =
                    parent.properties.GetProperty( "font-family" ).GetString();
                string fontStyle =
                    parent.properties.GetProperty( "font-style" ).GetString();
                string fontWeight =
                    parent.properties.GetProperty( "font-weight" ).GetString();
                int fontSize =
                    parent.properties.GetProperty( "font-size" ).GetLength().MValue();
                int fontVariant =
                    parent.properties.GetProperty( "font-variant" ).GetEnum();

                int letterSpacing =
                    parent.properties.GetProperty( "letter-spacing" ).GetLength().MValue();
                fs = new FontState( area.getFontInfo(), fontFamily,
                    fontStyle, fontWeight, fontSize,
                    fontVariant, letterSpacing );

                ColorType c = parent.properties.GetProperty( "color" ).GetColorType();
                red = c.Red;
                green = c.Green;
                blue = c.Blue;

                verticalAlign =
                    parent.properties.GetProperty( "vertical-align" ).GetEnum();

                wrapOption =
                    parent.properties.GetProperty( "wrap-option" ).GetEnum();
                whiteSpaceCollapse =
                    parent.properties.GetProperty( "white-space-collapse" ).GetEnum();
                ts = new TextState();
                ts.setUnderlined( underlined );
                ts.setOverlined( overlined );
                ts.setLineThrough( lineThrough );

                marker = start;
            }
            int orig_start = marker;
            marker = addText( (BlockArea)area, fs, red, green, blue,
                wrapOption, GetLinkSet(),
                whiteSpaceCollapse, ca, marker, length,
                ts, verticalAlign );
            if ( marker == -1 )
                return new Status( Status.OK );
            if ( marker != orig_start )
                return new Status( Status.AREA_FULL_SOME );
            return new Status( Status.AREA_FULL_NONE );
        }

        public static int addText( BlockArea ba, FontState fontState, float red,
            float green, float blue, int wrapOption,
            LinkSet ls, int whiteSpaceCollapse,
            char[] data, int start, int end,
            TextState textState, int vAlign )
        {
            if ( fontState.FontVariant == FontVariant.SMALL_CAPS )
            {
                FontState smallCapsFontState;
                try
                {
                    var smallCapsFontHeight =
                        (int)( fontState.FontSize * 0.8d );
                    smallCapsFontState = new FontState( fontState.FontInfo,
                        fontState.FontFamily,
                        fontState.FontStyle,
                        fontState.FontWeight,
                        smallCapsFontHeight,
                        FontVariant.NORMAL );
                }
                catch ( FonetException ex )
                {
                    smallCapsFontState = fontState;
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Error creating small-caps FontState: " + ex.Message );
                }

                char c;
                bool isLowerCase;
                int caseStart;
                FontState fontStateToUse;
                for ( int i = start; i < end; )
                {
                    caseStart = i;
                    c = data[ i ];
                    isLowerCase = char.IsLetter( c ) && char.IsLower( c );
                    while ( isLowerCase == ( char.IsLetter( c ) && char.IsLower( c ) ) )
                    {
                        if ( isLowerCase )
                            data[ i ] = char.ToUpper( c );
                        i++;
                        if ( i == end )
                            break;
                        c = data[ i ];
                    }
                    if ( isLowerCase )
                        fontStateToUse = smallCapsFontState;
                    else
                        fontStateToUse = fontState;
                    int index = addRealText( ba, fontStateToUse, red, green, blue,
                        wrapOption, ls, whiteSpaceCollapse,
                        data, caseStart, i, textState,
                        vAlign );
                    if ( index != -1 )
                        return index;
                }

                return -1;
            }

            return addRealText( ba, fontState, red, green, blue, wrapOption, ls,
                whiteSpaceCollapse, data, start, end, textState,
                vAlign );
        }

        protected static int addRealText( BlockArea ba, FontState fontState,
            float red, float green, float blue,
            int wrapOption, LinkSet ls,
            int whiteSpaceCollapse, char[] data,
            int start, int end, TextState textState,
            int vAlign )
        {
            int ts, te;
            char[] ca;

            ts = start;
            te = end;
            ca = data;

            LineArea la = ba.getCurrentLineArea();
            if ( la == null )
                return start;

            la.changeFont( fontState );
            la.changeColor( red, green, blue );
            la.changeWrapOption( wrapOption );
            la.changeWhiteSpaceCollapse( whiteSpaceCollapse );
            la.changeVerticalAlign( vAlign );
            ba.setupLinkSet( ls );

            ts = la.addText( ca, ts, te, ls, textState );

            while ( ts != -1 )
            {
                la = ba.createNextLineArea();
                if ( la == null )
                    return ts;
                la.changeFont( fontState );
                la.changeColor( red, green, blue );
                la.changeWrapOption( wrapOption );
                la.changeWhiteSpaceCollapse( whiteSpaceCollapse );
                ba.setupLinkSet( ls );

                ts = la.addText( ca, ts, te, ls, textState );
            }
            return -1;
        }
    }
}