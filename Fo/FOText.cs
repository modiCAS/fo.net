using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FoText : FoNode
    {
        private float _blue;
        protected char[] Ca;

        private FontState _fs;
        private float _green;
        protected int Length;
        protected bool LineThrough;
        protected bool Overlined;
        private float _red;
        protected int Start;

        private TextState _ts;

        protected bool Underlined;
        private int _verticalAlign;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public FoText( char[] chars, int s, int e, FObj parent )
            : base( parent )
        {
            Start = 0;
            Ca = new char[ e - s ];
            for ( int i = s; i < e; i++ )
                Ca[ i - s ] = chars[ i ];
            Length = e - s;
        }

        public void SetUnderlined( bool ul )
        {
            Underlined = ul;
        }

        public void SetOverlined( bool ol )
        {
            Overlined = ol;
        }

        public void SetLineThrough( bool lt )
        {
            LineThrough = lt;
        }

        public bool WillCreateArea()
        {
            _whiteSpaceCollapse =
                Parent.Properties.GetProperty( "white-space-collapse" ).GetEnum();
            if ( _whiteSpaceCollapse == GenericBoolean.Enums.False
                && Length > 0 )
                return true;

            for ( int i = Start; i < Start + Length; i++ )
            {
                char ch = Ca[ i ];
                if ( !( ch == ' ' || ch == '\n' || ch == '\r'
                    || ch == '\t' ) )
                    return true;
            }
            return false;
        }

        public override bool MayPrecedeMarker()
        {
            for ( var i = 0; i < Ca.Length; i++ )
            {
                char ch = Ca[ i ];
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
                    "Text outside block area" + new string( Ca, Start, Length ) );
                return new Status( Status.Ok );
            }
            if ( Marker == MarkerStart )
            {
                string fontFamily =
                    Parent.Properties.GetProperty( "font-family" ).GetString();
                string fontStyle =
                    Parent.Properties.GetProperty( "font-style" ).GetString();
                string fontWeight =
                    Parent.Properties.GetProperty( "font-weight" ).GetString();
                int fontSize =
                    Parent.Properties.GetProperty( "font-size" ).GetLength().MValue();
                int fontVariant =
                    Parent.Properties.GetProperty( "font-variant" ).GetEnum();

                int letterSpacing =
                    Parent.Properties.GetProperty( "letter-spacing" ).GetLength().MValue();
                _fs = new FontState( area.getFontInfo(), fontFamily,
                    fontStyle, fontWeight, fontSize,
                    fontVariant, letterSpacing );

                ColorType c = Parent.Properties.GetProperty( "color" ).GetColorType();
                _red = c.Red;
                _green = c.Green;
                _blue = c.Blue;

                _verticalAlign =
                    Parent.Properties.GetProperty( "vertical-align" ).GetEnum();

                _wrapOption =
                    Parent.Properties.GetProperty( "wrap-option" ).GetEnum();
                _whiteSpaceCollapse =
                    Parent.Properties.GetProperty( "white-space-collapse" ).GetEnum();
                _ts = new TextState();
                _ts.setUnderlined( Underlined );
                _ts.setOverlined( Overlined );
                _ts.setLineThrough( LineThrough );

                Marker = Start;
            }
            int origStart = Marker;
            Marker = AddText( (BlockArea)area, _fs, _red, _green, _blue,
                _wrapOption, GetLinkSet(),
                _whiteSpaceCollapse, Ca, Marker, Length,
                _ts, _verticalAlign );
            if ( Marker == -1 )
                return new Status( Status.Ok );
            if ( Marker != origStart )
                return new Status( Status.AreaFullSome );
            return new Status( Status.AreaFullNone );
        }

        public static int AddText( BlockArea ba, FontState fontState, float red,
            float green, float blue, int wrapOption,
            LinkSet ls, int whiteSpaceCollapse,
            char[] data, int start, int end,
            TextState textState, int vAlign )
        {
            if ( fontState.FontVariant == FontVariant.SmallCaps )
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
                        FontVariant.Normal );
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
                    int index = AddRealText( ba, fontStateToUse, red, green, blue,
                        wrapOption, ls, whiteSpaceCollapse,
                        data, caseStart, i, textState,
                        vAlign );
                    if ( index != -1 )
                        return index;
                }

                return -1;
            }

            return AddRealText( ba, fontState, red, green, blue, wrapOption, ls,
                whiteSpaceCollapse, data, start, end, textState,
                vAlign );
        }

        protected static int AddRealText( BlockArea ba, FontState fontState,
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