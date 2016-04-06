using System.Collections.Generic;
using System.Linq;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal sealed class FoText : FoNode
    {
        private float _blue;
        private readonly char[] _ca;

        private FontState _fs;
        private float _green;
        private readonly int _length;
        private bool _lineThrough;
        private bool _overlined;
        private float _red;
        private readonly int _start;

        private TextState _ts;

        private bool _underlined;
        private int _verticalAlign;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public FoText( IReadOnlyList<char> chars, int s, int e, FObj parent )
            : base( parent )
        {
            _start = 0;
            _ca = new char[ e - s ];
            for ( int i = s; i < e; i++ )
                _ca[ i - s ] = chars[ i ];
            _length = e - s;
        }

        public void SetUnderlined( bool ul )
        {
            _underlined = ul;
        }

        public void SetOverlined( bool ol )
        {
            _overlined = ol;
        }

        public void SetLineThrough( bool lt )
        {
            _lineThrough = lt;
        }

        public bool WillCreateArea()
        {
            _whiteSpaceCollapse =
                Parent.Properties.GetProperty( "white-space-collapse" ).GetEnum();
            if ( _whiteSpaceCollapse == GenericBoolean.Enums.False
                && _length > 0 )
                return true;

            for ( int i = _start; i < _start + _length; i++ )
            {
                char ch = _ca[ i ];
                if ( !( ch == ' ' || ch == '\n' || ch == '\r'
                    || ch == '\t' ) )
                    return true;
            }
            return false;
        }

        public override bool MayPrecedeMarker()
        {
            return _ca.Any( ch => ch != ' ' || ch != '\n' || ch != '\r' || ch != '\t' );
        }

        public override Status Layout( Area area )
        {
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Text outside block area" + new string( _ca, _start, _length ) );
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
                _fs = new FontState( area.GetFontInfo(), fontFamily,
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
                _ts.SetUnderlined( _underlined );
                _ts.SetOverlined( _overlined );
                _ts.SetLineThrough( _lineThrough );

                Marker = _start;
            }
            int origStart = Marker;
            Marker = AddText( (BlockArea)area, _fs, _red, _green, _blue,
                _wrapOption, GetLinkSet(),
                _whiteSpaceCollapse, _ca, Marker, _length,
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

                for ( int i = start; i < end; )
                {
                    int caseStart = i;
                    char c = data[ i ];
                    bool isLowerCase = char.IsLetter( c ) && char.IsLower( c );
                    while ( isLowerCase == ( char.IsLetter( c ) && char.IsLower( c ) ) )
                    {
                        if ( isLowerCase )
                            data[ i ] = char.ToUpper( c );
                        i++;
                        if ( i == end )
                            break;
                        c = data[ i ];
                    }
                    FontState fontStateToUse = isLowerCase ? smallCapsFontState : fontState;
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

        private static int AddRealText( BlockArea ba, FontState fontState,
            float red, float green, float blue,
            int wrapOption, LinkSet ls,
            int whiteSpaceCollapse, char[] data,
            int start, int end, TextState textState,
            int vAlign )
        {
            int ts = start;
            int te = end;
            char[] ca = data;

            LineArea la = ba.GetCurrentLineArea();
            if ( la == null )
                return start;

            la.ChangeFont( fontState );
            la.ChangeColor( red, green, blue );
            la.ChangeWrapOption( wrapOption );
            la.ChangeWhiteSpaceCollapse( whiteSpaceCollapse );
            la.ChangeVerticalAlign( vAlign );
            ba.SetupLinkSet( ls );

            ts = la.AddText( ca, ts, te, ls, textState );

            while ( ts != -1 )
            {
                la = ba.CreateNextLineArea();
                if ( la == null )
                    return ts;
                la.ChangeFont( fontState );
                la.ChangeColor( red, green, blue );
                la.ChangeWrapOption( wrapOption );
                la.ChangeWhiteSpaceCollapse( whiteSpaceCollapse );
                ba.SetupLinkSet( ls );

                ts = la.AddText( ca, ts, te, ls, textState );
            }
            return -1;
        }
    }
}