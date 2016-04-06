using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class PropertyManager
    {
        private static readonly string MsgColorFmt = "border-{0}-color";
        private static readonly string MsgStyleFmt = "border-{0}-style";
        private static readonly string MsgWidthFmt = "border-{0}-width";
        private static readonly string MsgPaddingFmt = "padding-{0}";
        private BackgroundProps _bgProps;
        private BorderAndPadding _borderAndPadding;
        private FontState _fontState;
        private HyphenationProps _hyphProps;
        private readonly PropertyList _properties;
        private string _saBottom;

        private string _saLeft;
        private string _saRight;
        private string _saTop;

        public PropertyManager( PropertyList pList )
        {
            _properties = pList;
        }

        private void InitDirections()
        {
            _saTop = _properties.WmAbsToRel( PropertyList.Top );
            _saBottom = _properties.WmAbsToRel( PropertyList.Bottom );
            _saLeft = _properties.WmAbsToRel( PropertyList.Left );
            _saRight = _properties.WmAbsToRel( PropertyList.Right );
        }

        public FontState GetFontState( FontInfo fontInfo )
        {
            if ( _fontState == null )
            {
                string fontFamily = _properties.GetProperty( "font-family" ).GetString();
                string fontStyle = _properties.GetProperty( "font-style" ).GetString();
                string fontWeight = _properties.GetProperty( "font-weight" ).GetString();
                int fontSize = _properties.GetProperty( "font-size" ).GetLength().MValue();
                int fontVariant = _properties.GetProperty( "font-variant" ).GetEnum();
                _fontState = new FontState( fontInfo, fontFamily, fontStyle,
                    fontWeight, fontSize, fontVariant );
            }
            return _fontState;
        }


        public BorderAndPadding GetBorderAndPadding()
        {
            if ( _borderAndPadding == null )
            {
                _borderAndPadding = new BorderAndPadding();
                InitDirections();

                InitBorderInfo( BorderAndPadding.Top, _saTop );
                InitBorderInfo( BorderAndPadding.Bottom, _saBottom );
                InitBorderInfo( BorderAndPadding.Left, _saLeft );
                InitBorderInfo( BorderAndPadding.Right, _saRight );
            }
            return _borderAndPadding;
        }

        private void InitBorderInfo( int whichSide, string saSide )
        {
            _borderAndPadding.SetPadding(
                whichSide, _properties.GetProperty( string.Format( MsgPaddingFmt, saSide ) ).GetCondLength() );
            int style = _properties.GetProperty( string.Format( MsgStyleFmt, saSide ) ).GetEnum();
            if ( style != Constants.None )
            {
                _borderAndPadding.SetBorder( whichSide, style,
                    _properties.GetProperty( string.Format( MsgWidthFmt, saSide ) ).GetCondLength(),
                    _properties.GetProperty( string.Format( MsgColorFmt, saSide ) ).GetColorType() );
            }
        }

        public HyphenationProps GetHyphenationProps()
        {
            if ( _hyphProps == null )
            {
                _hyphProps = new HyphenationProps();
                _hyphProps.Hyphenate = _properties.GetProperty( "hyphenate" ).GetEnum();
                _hyphProps.HyphenationChar =
                    _properties.GetProperty( "hyphenation-character" ).GetCharacter();
                _hyphProps.HyphenationPushCharacterCount =
                    _properties.GetProperty( "hyphenation-push-character-count" ).GetNumber().IntValue();
                _hyphProps.HyphenationRemainCharacterCount =
                    _properties.GetProperty( "hyphenation-remain-character-count" ).GetNumber().IntValue();
                _hyphProps.Language = _properties.GetProperty( "language" ).GetString();
                _hyphProps.Country = _properties.GetProperty( "country" ).GetString();
            }
            return _hyphProps;
        }

        public int CheckBreakBefore( Area area )
        {
            if ( !( area is ColumnArea ) )
            {
                switch ( _properties.GetProperty( "break-before" ).GetEnum() )
                {
                case GenericBreak.Enums.Page:
                    return Status.ForcePageBreak;
                case GenericBreak.Enums.OddPage:
                    return Status.ForcePageBreakOdd;
                case GenericBreak.Enums.EvenPage:
                    return Status.ForcePageBreakEven;
                case GenericBreak.Enums.Column:
                    return Status.ForceColumnBreak;
                default:
                    return Status.Ok;
                }
            }
            var colArea = (ColumnArea)area;
            switch ( _properties.GetProperty( "break-before" ).GetEnum() )
            {
            case GenericBreak.Enums.Page:
                if ( !colArea.HasChildren() && colArea.GetColumnIndex() == 1 )
                    return Status.Ok;
                return Status.ForcePageBreak;
            case GenericBreak.Enums.OddPage:
                if ( !colArea.HasChildren() && colArea.GetColumnIndex() == 1
                    && colArea.GetPage().GetNumber() % 2 != 0 )
                    return Status.Ok;
                return Status.ForcePageBreakOdd;
            case GenericBreak.Enums.EvenPage:
                if ( !colArea.HasChildren() && colArea.GetColumnIndex() == 1
                    && colArea.GetPage().GetNumber() % 2 == 0 )
                    return Status.Ok;
                return Status.ForcePageBreakEven;
            case GenericBreak.Enums.Column:
                if ( !area.HasChildren() )
                    return Status.Ok;
                return Status.ForceColumnBreak;
            default:
                return Status.Ok;
            }
        }

        public int CheckBreakAfter( Area area )
        {
            switch ( _properties.GetProperty( "break-after" ).GetEnum() )
            {
            case GenericBreak.Enums.Page:
                return Status.ForcePageBreak;
            case GenericBreak.Enums.OddPage:
                return Status.ForcePageBreakOdd;
            case GenericBreak.Enums.EvenPage:
                return Status.ForcePageBreakEven;
            case GenericBreak.Enums.Column:
                return Status.ForceColumnBreak;
            default:
                return Status.Ok;
            }
        }

        public MarginProps GetMarginProps()
        {
            var props = new MarginProps();

            props.MarginTop =
                _properties.GetProperty( "margin-top" ).GetLength().MValue();
            props.MarginBottom =
                _properties.GetProperty( "margin-bottom" ).GetLength().MValue();
            props.MarginLeft =
                _properties.GetProperty( "margin-left" ).GetLength().MValue();
            props.MarginRight =
                _properties.GetProperty( "margin-right" ).GetLength().MValue();
            return props;
        }

        public BackgroundProps GetBackgroundProps()
        {
            if ( _bgProps == null )
            {
                _bgProps = new BackgroundProps();

                _bgProps.BackColor =
                    _properties.GetProperty( "background-color" ).GetColorType();

                string src = _properties.GetProperty( "background-image" ).GetString();
                if ( src == "none" )
                    _bgProps.BackImage = null;
                else if ( src == "inherit" )
                    _bgProps.BackImage = null;
                else
                {
                    try
                    {
                        _bgProps.BackImage = FonetImageFactory.Make( src );
                    }
                    catch ( FonetImageException imgex )
                    {
                        _bgProps.BackImage = null;
                        FonetDriver.ActiveDriver.FireFonetError( imgex.Message );
                    }
                }

                _bgProps.BackRepeat = _properties.GetProperty( "background-repeat" ).GetEnum();
            }
            return _bgProps;
        }

        public MarginInlineProps GetMarginInlineProps()
        {
            var props = new MarginInlineProps();
            return props;
        }

        public AccessibilityProps GetAccessibilityProps()
        {
            var props = new AccessibilityProps();
            string str = _properties.GetProperty( "source-document" ).GetString();
            if ( !"none".Equals( str ) )
                props.SourceDoc = str;
            str = _properties.GetProperty( "role" ).GetString();
            if ( !"none".Equals( str ) )
                props.Role = str;
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

        public TextState GetTextDecoration( FObj parent )
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
                    tsp = fom.GetTextState();
                    found = true;
                }
                parent = parent.GetParent();
            }
            while ( !found );

            var ts = new TextState();

            if ( tsp != null )
            {
                ts.SetUnderlined( tsp.GetUnderlined() );
                ts.SetOverlined( tsp.GetOverlined() );
                ts.SetLineThrough( tsp.GetLineThrough() );
            }

            int textDecoration = _properties.GetProperty( "text-decoration" ).GetEnum();

            if ( textDecoration == TextDecoration.Underline )
                ts.SetUnderlined( true );
            if ( textDecoration == TextDecoration.Overline )
                ts.SetOverlined( true );
            if ( textDecoration == TextDecoration.LineThrough )
                ts.SetLineThrough( true );
            if ( textDecoration == TextDecoration.NoUnderline )
                ts.SetUnderlined( false );
            if ( textDecoration == TextDecoration.NoOverline )
                ts.SetOverlined( false );
            if ( textDecoration == TextDecoration.NoLineThrough )
                ts.SetLineThrough( false );

            return ts;
        }
    }
}