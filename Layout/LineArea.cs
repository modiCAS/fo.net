using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Fonet.Fo.Flow;
using Fonet.Fo.Properties;
using Fonet.Layout.Inline;
using Fonet.Render.Pdf;
using Fonet.Util;

namespace Fonet.Layout
{
    internal class LineArea : Area
    {
        private const int Nothing = 0;
        private const int Whitespace = 1;
        private const int Text = 2;
        private const int Multibytechar = 3;
        private int _allocationHeight;
        private FontState _currentFontState;
        private int _embeddedLinkStart;
        private int _endIndent;
        private int _finalWidth;
        private int _halfLeading;
        private HyphenationProps _hyphProps;
        private int _nominalFontSize;
        private ArrayList _pendingAreas = new ArrayList();
        private int _pendingWidth;
        private readonly int _placementOffset;
        private int _prev = Nothing;
        private bool _prevLtState;
        private bool _prevOlState;
        private bool _prevUlState;
        private float _red, _green, _blue;
        private int _spaceWidth;
        private int _startIndent;
        private int _vAlign;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public LineArea( FontState fontState, int lineHeight, int halfLeading,
            int allocationWidth, int startIndent, int endIndent,
            LineArea prevLineArea )
            : base( fontState )
        {
            _currentFontState = fontState;
            _nominalFontSize = fontState.FontSize;

            int nominalGlyphHeight = fontState.Ascender - fontState.Descender;

            _placementOffset = fontState.Ascender;
            ContentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            FontState = fontState;

            _allocationHeight = nominalGlyphHeight;
            _halfLeading = lineHeight - _allocationHeight;

            _startIndent = startIndent;
            _endIndent = endIndent;

            if ( prevLineArea == null ) return;

            IEnumerator e = prevLineArea._pendingAreas.GetEnumerator();
            Box b = null;
            var eatMoreSpace = true;
            var eatenWidth = 0;

            while ( eatMoreSpace )
            {
                if ( e.MoveNext() )
                {
                    b = (Box)e.Current;
                    var space = b as InlineSpace;
                    if ( space != null )
                    {
                        InlineSpace isp = space;
                        if ( isp.IsEatable() )
                            eatenWidth += isp.GetSize();
                        else
                            eatMoreSpace = false;
                    }
                    else
                        eatMoreSpace = false;
                }
                else
                {
                    eatMoreSpace = false;
                    b = null;
                }
            }

            while ( b != null )
            {
                _pendingAreas.Add( b );
                b = e.MoveNext() ? (Box)e.Current : null;
            }

            _pendingWidth = prevLineArea.GetPendingWidth() - eatenWidth;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderLineArea( this );
        }

        public int AddPageNumberCitation( string refid, LinkSet ls )
        {
            int width = _currentFontState.GetWidth( _currentFontState.MapCharacter( ' ' ) );


            var pia = new PageNumberInlineArea( _currentFontState,
                _red, _green, _blue, refid, width );

            pia.SetYOffset( _placementOffset );
            _pendingAreas.Add( pia );
            _pendingWidth += width;
            _prev = Text;

            return -1;
        }

        public int AddText( char[] odata, int start, int end, LinkSet ls,
            TextState textState )
        {
            if ( start == -1 )
                return -1;
            var overrun = false;

            int wordStart = start;
            var wordLength = 0;
            var wordWidth = 0;
            int whitespaceWidth = GetCharWidth( ' ' );

            var data = new char[ odata.Length ];
            var dataCopy = new char[ odata.Length ];
            Array.Copy( odata, data, odata.Length );
            Array.Copy( odata, dataCopy, odata.Length );

            for ( int i = start; i < end; i++ )
            {
                int charWidth;
                char c = data[ i ];
                bool isMultiByteChar;
                bool isText;
                if ( !( IsSpace( c ) || c == '\n' || c == '\r' || c == '\t'
                    || c == '\u2028' ) )
                {
                    charWidth = GetCharWidth( c );
                    isText = true;
                    isMultiByteChar = c > 127;
                    if ( charWidth <= 0 && c != '\u200B' && c != '\uFEFF' )
                        charWidth = whitespaceWidth;
                }
                else
                {
                    if ( c == '\n' || c == '\r' || c == '\t' )
                        charWidth = whitespaceWidth;
                    else
                        charWidth = GetCharWidth( c );

                    isText = false;
                    isMultiByteChar = false;

                    if ( _prev == Whitespace )
                    {
                        if ( _whiteSpaceCollapse == GenericBoolean.Enums.False )
                        {
                            if ( IsSpace( c ) )
                                _spaceWidth += GetCharWidth( c );
                            else
                                switch ( c )
                                {
                                case '\n':
                                case '\u2028':
                                    if ( _spaceWidth <= 0 ) return i + 1;

                                    var isp = new InlineSpace( _spaceWidth );
                                    isp.SetUnderlined( textState.GetUnderlined() );
                                    isp.SetOverlined( textState.GetOverlined() );
                                    isp.SetLineThrough( textState.GetLineThrough() );
                                    AddChild( isp );
                                    _finalWidth += _spaceWidth;
                                    _spaceWidth = 0;
                                    return i + 1;
                                case '\t':
                                    _spaceWidth += 8 * whitespaceWidth;
                                    break;
                                }
                        }
                        else if ( c == '\u2028' )
                        {
                            if ( _spaceWidth <= 0 ) return i + 1;

                            var isp = new InlineSpace( _spaceWidth );
                            isp.SetUnderlined( textState.GetUnderlined() );
                            isp.SetOverlined( textState.GetOverlined() );
                            isp.SetLineThrough( textState.GetLineThrough() );
                            AddChild( isp );
                            _finalWidth += _spaceWidth;
                            _spaceWidth = 0;
                            return i + 1;
                        }
                    }
                    else if ( _prev == Text || _prev == Multibytechar )
                    {
                        if ( _spaceWidth > 0 )
                        {
                            var isp = new InlineSpace( _spaceWidth );
                            if ( _prevUlState )
                                isp.SetUnderlined( textState.GetUnderlined() );
                            if ( _prevOlState )
                                isp.SetOverlined( textState.GetOverlined() );
                            if ( _prevLtState )
                                isp.SetLineThrough( textState.GetLineThrough() );
                            AddChild( isp );
                            _finalWidth += _spaceWidth;
                            _spaceWidth = 0;
                        }

                        IEnumerator e = _pendingAreas.GetEnumerator();
                        while ( e.MoveNext() )
                        {
                            var box = (Box)e.Current;
                            var area = box as InlineArea;
                            if ( area != null )
                            {
                                if ( ls != null )
                                {
                                    var lr =
                                        new Rectangle( _finalWidth, 0,
                                            area.GetContentWidth(),
                                            FontState.FontSize );
                                    ls.AddRect( lr, this, area );
                                }
                            }

                            AddChild( box );
                        }

                        _finalWidth += _pendingWidth;

                        _pendingWidth = 0;
                        _pendingAreas = new ArrayList();

                        if ( wordLength > 0 )
                        {
                            AddSpacedWord( new string( data, wordStart, wordLength ),
                                ls, _finalWidth, 0, textState, false );
                            _finalWidth += wordWidth;

                            wordWidth = 0;
                        }

                        _prev = Whitespace;

                        _embeddedLinkStart = 0;

                        _spaceWidth = GetCharWidth( c );

                        if ( _whiteSpaceCollapse == GenericBoolean.Enums.False )
                        {
                            switch ( c )
                            {
                            case '\n':
                            case '\u2028':
                                return i + 1;

                            case '\t':
                                _spaceWidth = whitespaceWidth;
                                break;
                            }
                        }
                        else if ( c == '\u2028' )
                            return i + 1;
                    }
                    else
                    {
                        if ( _whiteSpaceCollapse == GenericBoolean.Enums.False )
                        {
                            if ( IsSpace( c ) )
                            {
                                _prev = Whitespace;
                                _spaceWidth = GetCharWidth( c );
                            }
                            else
                                switch ( c )
                                {
                                case '\n':
                                    var isp = new InlineSpace( _spaceWidth );
                                    AddChild( isp );
                                    return i + 1;

                                case '\t':
                                    _prev = Whitespace;
                                    _spaceWidth = 8 * whitespaceWidth;
                                    break;
                                }
                        }
                        else
                            wordStart++;
                    }
                }

                if ( !isText ) continue;

                int curr = isMultiByteChar ? Multibytechar : Text;
                switch ( _prev )
                {
                case Whitespace:
                    wordWidth = charWidth;
                    if ( _finalWidth + _spaceWidth + wordWidth
                        > GetContentWidth() )
                    {
                        if ( overrun )
                        {
                            FonetDriver.ActiveDriver.FireFonetWarning(
                                "Area contents overflows area" );
                        }
                        if ( _wrapOption == WrapOption.Wrap )
                            return i;
                    }
                    _prev = curr;
                    wordStart = i;
                    wordLength = 1;
                    break;
                case Text:
                case Multibytechar:
                    if ( _prev == Text && curr == Text || !CanBreakMidWord() )
                    {
                        wordLength++;
                        wordWidth += charWidth;
                    }
                    else
                    {
                        var isp = new InlineSpace( _spaceWidth );
                        if ( _prevUlState )
                            isp.SetUnderlined( textState.GetUnderlined() );
                        if ( _prevOlState )
                            isp.SetOverlined( textState.GetOverlined() );
                        if ( _prevLtState )
                            isp.SetLineThrough( textState.GetLineThrough() );
                        AddChild( isp );
                        _finalWidth += _spaceWidth;
                        _spaceWidth = 0;

                        IEnumerator e = _pendingAreas.GetEnumerator();
                        while ( e.MoveNext() )
                        {
                            var box = (Box)e.Current;
                            var area = box as InlineArea;
                            if ( area != null )
                            {
                                if ( ls != null )
                                {
                                    var lr =
                                        new Rectangle( _finalWidth, 0,
                                            area.GetContentWidth(),
                                            FontState.FontSize );
                                    ls.AddRect( lr, this, area );
                                }
                            }
                            AddChild( box );
                        }

                        _finalWidth += _pendingWidth;

                        _pendingWidth = 0;
                        _pendingAreas = new ArrayList();

                        if ( wordLength > 0 )
                        {
                            AddSpacedWord( new string( data, wordStart, wordLength ),
                                ls, _finalWidth, 0, textState, false );
                            _finalWidth += wordWidth;
                        }
                        _spaceWidth = 0;
                        wordStart = i;
                        wordLength = 1;
                        wordWidth = charWidth;
                    }
                    _prev = curr;
                    break;
                default:
                    _prev = curr;
                    wordStart = i;
                    wordLength = 1;
                    wordWidth = charWidth;
                    break;
                }

                if ( _finalWidth + _spaceWidth + _pendingWidth + wordWidth <= GetContentWidth() ) continue;
                if ( _wrapOption != WrapOption.Wrap ) continue;

                if ( wordStart == start )
                {
                    overrun = true;
                    if ( _finalWidth > 0 ) return wordStart;
                }
                else return wordStart;
            }

            if ( _prev == Text || _prev == Multibytechar )
            {
                if ( _spaceWidth > 0 )
                {
                    var pis = new InlineSpace( _spaceWidth );
                    pis.SetEatable( true );
                    if ( _prevUlState )
                        pis.SetUnderlined( textState.GetUnderlined() );
                    if ( _prevOlState )
                        pis.SetOverlined( textState.GetOverlined() );
                    if ( _prevLtState )
                        pis.SetLineThrough( textState.GetLineThrough() );
                    _pendingAreas.Add( pis );
                    _pendingWidth += _spaceWidth;
                    _spaceWidth = 0;
                }

                AddSpacedWord( new string( data, wordStart, wordLength ), ls,
                    _finalWidth + _pendingWidth,
                    _spaceWidth, textState, true );

                _embeddedLinkStart += wordWidth;
            }

            if ( overrun )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Area contents overflows area" );
            }

            return -1;
        }

        public void AddLeader( LeaderPattern leaderPattern, int leaderLengthMinimum,
            int leaderLengthOptimum, int leaderLengthMaximum,
            int ruleStyle, int ruleThickness,
            int leaderPatternWidth, int leaderAlignment )
        {
            var leaderLength = 0;
            const char dotIndex = '.';
            int dotWidth = _currentFontState.GetWidth( _currentFontState.MapCharacter( dotIndex ) );
            const char whitespaceIndex = ' ';
            int whitespaceWidth = _currentFontState.GetWidth( _currentFontState.MapCharacter( whitespaceIndex ) );

            int remainingWidth = GetRemainingWidth();

            if ( remainingWidth <= leaderLengthOptimum || remainingWidth <= leaderLengthMaximum )
                leaderLength = remainingWidth;
            else if ( remainingWidth > leaderLengthOptimum && remainingWidth > leaderLengthMaximum )
                leaderLength = leaderLengthMaximum;
            else if ( leaderLengthOptimum > leaderLengthMaximum && leaderLengthOptimum < remainingWidth )
                leaderLength = leaderLengthOptimum;

            if ( leaderLength <= 0 ) return;

            switch ( leaderPattern )
            {
            case LeaderPattern.Space:
                var spaceArea = new InlineSpace( leaderLength );
                _pendingAreas.Add( spaceArea );
                break;

            case LeaderPattern.Rule:
                var leaderArea = new LeaderArea( FontState, _red, _green, _blue,
                    string.Empty, leaderLength, leaderPattern, ruleThickness, ruleStyle );
                leaderArea.SetYOffset( _placementOffset );
                _pendingAreas.Add( leaderArea );
                break;

            case LeaderPattern.Dots:
                if ( leaderPatternWidth < dotWidth ) leaderPatternWidth = 0;
                if ( leaderPatternWidth == 0 )
                {
                    _pendingAreas.Add( BuildSimpleLeader( dotIndex, leaderLength ) );
                }
                else
                {
                    if ( leaderAlignment == LeaderAlignment.ReferenceArea )
                    {
                        int spaceBeforeLeader =
                            GetLeaderAlignIndent( leaderLength,
                                leaderPatternWidth );
                        if ( spaceBeforeLeader != 0 )
                        {
                            _pendingAreas.Add( new InlineSpace( spaceBeforeLeader,
                                false ) );
                            _pendingWidth += spaceBeforeLeader;
                            leaderLength -= spaceBeforeLeader;
                        }
                    }

                    var spaceBetweenDots = new InlineSpace( leaderPatternWidth - dotWidth, false );

                    var leaderPatternArea = new WordArea( _currentFontState, _red,
                        _green, _blue, ".", dotWidth );
                    leaderPatternArea.SetYOffset( _placementOffset );
                    var dotsFactor = (int)Math.Floor( leaderLength / (double)leaderPatternWidth );

                    for ( var i = 0; i < dotsFactor; i++ )
                    {
                        _pendingAreas.Add( leaderPatternArea );
                        _pendingAreas.Add( spaceBetweenDots );
                    }

                    _pendingAreas.Add( new InlineSpace( leaderLength - dotsFactor * leaderPatternWidth ) );
                }
                break;

            case LeaderPattern.UseContent:
                FonetDriver.ActiveDriver.FireFonetError(
                    "leader-pattern=\"use-content\" not supported by this version of FO.NET" );
                return;
            }

            _pendingWidth += leaderLength;
            _prev = Text;
        }

        public void AddPending()
        {
            if ( _spaceWidth > 0 )
            {
                AddChild( new InlineSpace( _spaceWidth ) );
                _finalWidth += _spaceWidth;
                _spaceWidth = 0;
            }

            foreach ( Box box in _pendingAreas )
                AddChild( box );

            _finalWidth += _pendingWidth;

            _pendingWidth = 0;
            _pendingAreas = new ArrayList();
        }

        public void Align( int type )
        {
            int padding;

            switch ( type )
            {
            case TextAlign.Start:
                padding = GetContentWidth() - _finalWidth;
                _endIndent += padding;
                break;
            case TextAlign.End:
                padding = GetContentWidth() - _finalWidth;
                _startIndent += padding;
                break;
            case TextAlign.Center:
                padding = ( GetContentWidth() - _finalWidth ) / 2;
                _startIndent += padding;
                _endIndent += padding;
                break;
            case TextAlign.Justify:
                int spaceCount = Children.OfType<InlineSpace>().Count( space => space.GetResizeable() );

                padding = spaceCount > 0 ? ( GetContentWidth() - _finalWidth ) / spaceCount : 0;
                spaceCount = 0;

                foreach ( Box b in Children )
                {
                    var space = b as InlineSpace;
                    if ( space != null )
                    {
                        if ( !space.GetResizeable() ) continue;

                        space.SetSize( space.GetSize() + padding );
                        spaceCount++;
                    }
                    else if ( b is InlineArea )
                    {
                        ( (InlineArea)b ).SetXOffset( spaceCount * padding );
                    }
                }
                break;
            }
        }

        public void VerticalAlign()
        {
            int superHeight = -_placementOffset;
            int maxHeight = _allocationHeight;

            foreach ( InlineArea ia in Children.OfType<InlineArea>() )
            {
                if ( ia is WordArea ) ia.SetYOffset( _placementOffset );
                if ( ia.GetHeight() > maxHeight ) maxHeight = ia.GetHeight();

                int vert = ia.GetVerticalAlign();
                switch ( vert )
                {
                case Fo.Properties.VerticalAlign.Super:
                    {
                    int fh = FontState.Ascender;
                    ia.SetYOffset( (int)( _placementOffset - 2 * fh / 3.0 ) );
                    }
                    break;

                case Fo.Properties.VerticalAlign.Sub:
                    {
                    int fh = FontState.Ascender;
                    ia.SetYOffset( (int)( _placementOffset + 2 * fh / 3.0 ) );
                    }
                    break;
                }
            }
            _allocationHeight = maxHeight;
        }

        public void ChangeColor( float red, float green, float blue )
        {
            _red = red;
            _green = green;
            _blue = blue;
        }

        public void ChangeFont( FontState fontState )
        {
            _currentFontState = fontState;
        }

        public void ChangeWhiteSpaceCollapse( int whiteSpaceCollapse )
        {
            _whiteSpaceCollapse = whiteSpaceCollapse;
        }

        public void ChangeWrapOption( int wrapOption )
        {
            _wrapOption = wrapOption;
        }

        public void ChangeVerticalAlign( int vAlign )
        {
            _vAlign = vAlign;
        }

        public int GetEndIndent()
        {
            return _endIndent;
        }

        public override int GetHeight()
        {
            return _allocationHeight;
        }

        public int GetPlacementOffset()
        {
            return _placementOffset;
        }

        public int GetStartIndent()
        {
            return _startIndent;
        }

        public bool IsEmpty()
        {
            return !( _pendingAreas.Count > 0 || Children.Count > 0 );
        }

        public ArrayList GetPendingAreas()
        {
            return _pendingAreas;
        }

        public int GetPendingWidth()
        {
            return _pendingWidth;
        }

        public void SetPendingAreas( ArrayList areas )
        {
            _pendingAreas = areas;
        }

        public void SetPendingWidth( int width )
        {
            _pendingWidth = width;
        }

        public void ChangeHyphenation( HyphenationProps hyphProps )
        {
            _hyphProps = hyphProps;
        }

        private InlineArea BuildSimpleLeader( char c, int leaderLength )
        {
            int width = _currentFontState.GetWidth( _currentFontState.MapCharacter( c ) );
            if ( width == 0 )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "char '" + c + "' has width 0. Using width 100 instead." );
                width = 100;
            }
            var factor = (int)Math.Floor( (decimal)leaderLength / width );
            var leaderChars = new char[ factor ];
            for ( var i = 0; i < factor; i++ )
                leaderChars[ i ] = c;
            var leaderPatternArea = new WordArea( _currentFontState, _red,
                _green, _blue,
                new string( leaderChars ),
                leaderLength );
            leaderPatternArea.SetYOffset( _placementOffset );
            return leaderPatternArea;
        }

        private int GetLeaderAlignIndent( int leaderLength, int leaderPatternWidth )
        {
            double position = GetCurrentXPosition();
            double nextRepeatedLeaderPatternCycle = Math.Ceiling( position
                / leaderPatternWidth );
            double difference =
                leaderPatternWidth * nextRepeatedLeaderPatternCycle - position;
            return (int)difference;
        }

        private int GetCurrentXPosition()
        {
            return _finalWidth + _spaceWidth + _startIndent + _pendingWidth;
        }

        private string GetHyphenationWord( IReadOnlyList<char> characters, int wordStart )
        {
            var wordendFound = false;
            var counter = 0;
            var newWord = new char[ characters.Count ];
            while ( !wordendFound
                && wordStart + counter < characters.Count )
            {
                char tk = characters[ wordStart + counter ];
                if ( char.IsLetter( tk ) )
                {
                    newWord[ counter ] = tk;
                    counter++;
                }
                else
                    wordendFound = true;
            }
            return new string( newWord, 0, counter );
        }

        private int GetWordWidth( string word )
        {
            return word == null ? 0 : word.Sum( c => GetCharWidth( c ) );
        }

        public int GetRemainingWidth()
        {
            return GetContentWidth() + _startIndent - GetCurrentXPosition();
        }

        public void SetLinkSet( LinkSet ls )
        {
        }

        public void AddInlineArea( InlineArea box, LinkSet ls )
        {
            AddPending();
            AddChild( box );
            if ( ls != null )
            {
                var lr = new Rectangle( _finalWidth, 0, box.GetContentWidth(), box.GetContentHeight() );
                ls.AddRect( lr, this, box );
            }

            _prev = Text;
            _finalWidth += box.GetContentWidth();
        }

        public void AddInlineSpace( InlineSpace isp, int spaceWidth )
        {
            AddChild( isp );
            _finalWidth += spaceWidth;
        }

        public int AddCharacter( char data, LinkSet ls, bool ul )
        {
            int remainingWidth = GetRemainingWidth();
            int width = _currentFontState.GetWidth( _currentFontState.MapCharacter( data ) );
            if ( width > remainingWidth ) return Character.DoesnotFit;
            if ( char.IsWhiteSpace( data ) && _whiteSpaceCollapse == GenericBoolean.Enums.True ) return Character.Ok;

            var ia = new WordArea( _currentFontState, _red, _green,
                _blue, data.ToString(), width );
            ia.SetYOffset( _placementOffset );
            ia.SetUnderlined( ul );
            _pendingAreas.Add( ia );

            if ( char.IsWhiteSpace( data ) )
            {
                _spaceWidth = +width;
                _prev = Whitespace;
            }
            else
            {
                _pendingWidth += width;
                _prev = Text;
            }

            return Character.Ok;
        }

        private void AddMapWord( char startChar, StringBuilder wordBuf )
        {
            var mapBuf = new StringBuilder( wordBuf.Length );
            for ( var i = 0; i < wordBuf.Length; i++ )
                mapBuf.Append( _currentFontState.MapCharacter( wordBuf[ i ] ) );

            AddWord( startChar, mapBuf );
        }

        private void AddWord( char startChar, StringBuilder wordBuf )
        {
            string word = wordBuf != null ? wordBuf.ToString() : "";
            WordArea hia;
            int startCharWidth = GetCharWidth( startChar );

            if ( IsAnySpace( startChar ) ) AddChild( new InlineSpace( startCharWidth ) );
            else
            {
                hia = new WordArea( _currentFontState, _red, _green,
                    _blue,
                    startChar.ToString(), 1 );
                hia.SetYOffset( _placementOffset );
                AddChild( hia );
            }

            int wordWidth = GetWordWidth( word );
            hia = new WordArea( _currentFontState, _red, _green, _blue, word, word.Length );
            hia.SetYOffset( _placementOffset );
            AddChild( hia );

            _finalWidth += startCharWidth + wordWidth;
        }

        private bool CanBreakMidWord()
        {
            if ( _hyphProps == null || _hyphProps.Language == null || _hyphProps.Language.Equals( "NONE" ) ) return false;

            string lang = _hyphProps.Language.ToLower();
            return "zh".Equals( lang ) || "ja".Equals( lang ) || "ko".Equals( lang ) || "vi".Equals( lang );
        }

        private int GetCharWidth( char c )
        {
            float factor = 1;
            switch ( c )
            {
            case '\n':
            case '\r':
            case '\t':
            case '\u00A0':
            case '\u2007':
                c = ' ';
                break;

            case '\u2008':
                c = '.';
                break;

            case '\u202F':
                c = ' ';
                factor = 0.5f;
                break;

            case '\u3000':
                c = ' ';
                factor = 2;
                break;
            }

            int width = _currentFontState.GetWidth( _currentFontState.MapCharacter( c ) );
            if ( width > 0 ) return width;

            int em = _currentFontState.GetWidth( _currentFontState.MapCharacter( 'm' ) );
            int en = _currentFontState.GetWidth( _currentFontState.MapCharacter( 'n' ) );
            if ( em <= 0 ) em = 500 * _currentFontState.FontSize;
            if ( en <= 0 ) en = em - 10;

            switch ( c )
            {
            case ' ':
                return (int)( em * factor );
            case '\u2000':
                return en;
            case '\u2001':
                return em;
            case '\u2002':
                return em / 2;
            case '\u2003':
                return _currentFontState.FontSize;
            case '\u2004':
                return em / 3;
            case '\u2005':
                return em / 4;
            case '\u2006':
                return em / 6;
            case '\u2009':
                return em / 5;
            case '\u200A':
                return 5;
            case '\u200B':
                return 100;
            }

            return 0;
        }

        private static bool IsSpace( char c )
        {
            return c == ' ' ||
                c == '\u2000' || // en quad
                c == '\u2001' || // em quad
                c == '\u2002' || // en space
                c == '\u2003' || // em space
                c == '\u2004' || // three-per-em space
                c == '\u2005' || // four--per-em space
                c == '\u2006' || // six-per-em space
                c == '\u2007' || // figure space
                c == '\u2008' || // punctuation space
                c == '\u2009' || // thin space
                c == '\u200A' || // hair space
                c == '\u200B';
        }

        private static bool IsNbsp( char c )
        {
            return c == '\u00A0' ||
                c == '\u202F' || // narrow no-break space
                c == '\u3000' || // ideographic space
                c == '\uFEFF';
        }

        private static bool IsAnySpace( char c )
        {
            return IsSpace( c ) || IsNbsp( c );
        }

        private void AddSpacedWord( string word, LinkSet ls, int startw,
            int spacew, TextState textState, bool addToPending )
        {
            /*
             * Split string based on four delimeters:
             * \u00A0 - Latin1 NBSP (Non breaking space)
             * \u202F - unknown reserved character according to Unicode Standard
             * \u3000 - CJK IDSP (Ideographic space)
             * \uFEFF - Arabic ZWN BSP (Zero width no break space)
             */
            var st = new StringTokenizer( word, "\u00A0\u202F\u3000\uFEFF", true );
            var extraw = 0;
            while ( st.MoveNext() )
            {
                var currentWord = (string)st.Current;

                if ( currentWord.Length == 1
                    && IsNbsp( currentWord[ 0 ] ) )
                {
                    // Add an InlineSpace
                    int spaceWidth = GetCharWidth( currentWord[ 0 ] );
                    if ( spaceWidth <= 0 ) continue;

                    var ispace = new InlineSpace( spaceWidth );
                    extraw += spaceWidth;
                    if ( _prevUlState )
                        ispace.SetUnderlined( textState.GetUnderlined() );
                    if ( _prevOlState )
                        ispace.SetOverlined( textState.GetOverlined() );
                    if ( _prevLtState )
                        ispace.SetLineThrough( textState.GetLineThrough() );

                    if ( addToPending )
                    {
                        _pendingAreas.Add( ispace );
                        _pendingWidth += spaceWidth;
                    }
                    else AddChild( ispace );
                }
                else
                {
                    var ia = new WordArea( _currentFontState, _red,
                        _green, _blue,
                        currentWord,
                        GetWordWidth( currentWord ) );
                    ia.SetYOffset( _placementOffset );
                    ia.SetUnderlined( textState.GetUnderlined() );
                    _prevUlState = textState.GetUnderlined();
                    ia.SetOverlined( textState.GetOverlined() );
                    _prevOlState = textState.GetOverlined();
                    ia.SetLineThrough( textState.GetLineThrough() );
                    _prevLtState = textState.GetLineThrough();
                    ia.SetVerticalAlign( _vAlign );

                    if ( addToPending )
                    {
                        _pendingAreas.Add( ia );
                        _pendingWidth += GetWordWidth( currentWord );
                    }
                    else AddChild( ia );
                    if ( ls == null ) continue;

                    var lr = new Rectangle( startw + extraw, spacew,
                        ia.GetContentWidth(),
                        FontState.FontSize );
                    ls.AddRect( lr, this, ia );
                }
            }
        }
    }
}