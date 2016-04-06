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

                InitBorderInfo( BorderAndPadding.TOP, _saTop );
                InitBorderInfo( BorderAndPadding.BOTTOM, _saBottom );
                InitBorderInfo( BorderAndPadding.LEFT, _saLeft );
                InitBorderInfo( BorderAndPadding.RIGHT, _saRight );
            }
            return _borderAndPadding;
        }

        private void InitBorderInfo( int whichSide, string saSide )
        {
            _borderAndPadding.setPadding(
                whichSide, _properties.GetProperty( string.Format( MsgPaddingFmt, saSide ) ).GetCondLength() );
            int style = _properties.GetProperty( string.Format( MsgStyleFmt, saSide ) ).GetEnum();
            if ( style != Constants.None )
            {
                _borderAndPadding.setBorder( whichSide, style,
                    _properties.GetProperty( string.Format( MsgWidthFmt, saSide ) ).GetCondLength(),
                    _properties.GetProperty( string.Format( MsgColorFmt, saSide ) ).GetColorType() );
            }
        }

        public HyphenationProps GetHyphenationProps()
        {
            if ( _hyphProps == null )
            {
                _hyphProps = new HyphenationProps();
                _hyphProps.hyphenate = _properties.GetProperty( "hyphenate" ).GetEnum();
                _hyphProps.hyphenationChar =
                    _properties.GetProperty( "hyphenation-character" ).GetCharacter();
                _hyphProps.hyphenationPushCharacterCount =
                    _properties.GetProperty( "hyphenation-push-character-count" ).GetNumber().IntValue();
                _hyphProps.hyphenationRemainCharacterCount =
                    _properties.GetProperty( "hyphenation-remain-character-count" ).GetNumber().IntValue();
                _hyphProps.language = _properties.GetProperty( "language" ).GetString();
                _hyphProps.country = _properties.GetProperty( "country" ).GetString();
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
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1 )
                    return Status.Ok;
                return Status.ForcePageBreak;
            case GenericBreak.Enums.OddPage:
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1
                    && colArea.getPage().getNumber() % 2 != 0 )
                    return Status.Ok;
                return Status.ForcePageBreakOdd;
            case GenericBreak.Enums.EvenPage:
                if ( !colArea.hasChildren() && colArea.getColumnIndex() == 1
                    && colArea.getPage().getNumber() % 2 == 0 )
                    return Status.Ok;
                return Status.ForcePageBreakEven;
            case GenericBreak.Enums.Column:
                if ( !area.hasChildren() )
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

            props.marginTop =
                _properties.GetProperty( "margin-top" ).GetLength().MValue();
            props.marginBottom =
                _properties.GetProperty( "margin-bottom" ).GetLength().MValue();
            props.marginLeft =
                _properties.GetProperty( "margin-left" ).GetLength().MValue();
            props.marginRight =
                _properties.GetProperty( "margin-right" ).GetLength().MValue();
            return props;
        }

        public BackgroundProps GetBackgroundProps()
        {
            if ( _bgProps == null )
            {
                _bgProps = new BackgroundProps();

                _bgProps.backColor =
                    _properties.GetProperty( "background-color" ).GetColorType();

                string src = _properties.GetProperty( "background-image" ).GetString();
                if ( src == "none" )
                    _bgProps.backImage = null;
                else if ( src == "inherit" )
                    _bgProps.backImage = null;
                else
                {
                    try
                    {
                        _bgProps.backImage = FonetImageFactory.Make( src );
                    }
                    catch ( FonetImageException imgex )
                    {
                        _bgProps.backImage = null;
                        FonetDriver.ActiveDriver.FireFonetError( imgex.Message );
                    }
                }

                _bgProps.backRepeat = _properties.GetProperty( "background-repeat" ).GetEnum();
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
            string str;
            str = _properties.GetProperty( "source-document" ).GetString();
            if ( !"none".Equals( str ) )
                props.sourceDoc = str;
            str = _properties.GetProperty( "role" ).GetString();
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
                ts.setUnderlined( tsp.getUnderlined() );
                ts.setOverlined( tsp.getOverlined() );
                ts.setLineThrough( tsp.getLineThrough() );
            }

            int textDecoration = _properties.GetProperty( "text-decoration" ).GetEnum();

            if ( textDecoration == TextDecoration.Underline )
                ts.setUnderlined( true );
            if ( textDecoration == TextDecoration.Overline )
                ts.setOverlined( true );
            if ( textDecoration == TextDecoration.LineThrough )
                ts.setLineThrough( true );
            if ( textDecoration == TextDecoration.NoUnderline )
                ts.setUnderlined( false );
            if ( textDecoration == TextDecoration.NoOverline )
                ts.setOverlined( false );
            if ( textDecoration == TextDecoration.NoLineThrough )
                ts.setLineThrough( false );

            return ts;
        }
    }
}