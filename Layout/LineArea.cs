using System;
using System.Collections;
using System.Drawing;
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
        protected const int Nothing = 0;
        protected const int Whitespace = 1;
        protected const int Text = 2;
        protected const int Multibytechar = 3;
        protected int AllocationHeight;
        private FontState _currentFontState;
        protected int EmbeddedLinkStart;
        protected int EndIndent;
        protected int FinalWidth;
        protected int HalfLeading;
        private HyphenationProps _hyphProps;
        protected int LineHeight;
        protected int NominalFontSize;
        protected int NominalGlyphHeight;
        protected ArrayList PendingAreas = new ArrayList();
        protected int PendingWidth;
        private readonly int _placementOffset;
        protected int Prev = Nothing;
        protected bool PrevLtState;
        protected bool PrevOlState;
        protected bool PrevUlState;
        private float _red, _green, _blue;
        protected int SpaceWidth;
        protected int StartIndent;
        private int _vAlign;
        private int _whiteSpaceCollapse;
        private int _wrapOption;

        public LineArea( FontState fontState, int lineHeight, int halfLeading,
            int allocationWidth, int startIndent, int endIndent,
            LineArea prevLineArea )
            : base( fontState )
        {
            _currentFontState = fontState;
            this.LineHeight = lineHeight;
            NominalFontSize = fontState.FontSize;
            NominalGlyphHeight = fontState.Ascender - fontState.Descender;

            _placementOffset = fontState.Ascender;
            ContentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            this.FontState = fontState;

            AllocationHeight = NominalGlyphHeight;
            this.HalfLeading = this.LineHeight - AllocationHeight;

            this.StartIndent = startIndent;
            this.EndIndent = endIndent;

            if ( prevLineArea != null )
            {
                IEnumerator e = prevLineArea.PendingAreas.GetEnumerator();
                Box b = null;
                var eatMoreSpace = true;
                var eatenWidth = 0;

                while ( eatMoreSpace )
                {
                    if ( e.MoveNext() )
                    {
                        b = (Box)e.Current;
                        if ( b is InlineSpace )
                        {
                            var isp = (InlineSpace)b;
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
                    PendingAreas.Add( b );
                    if ( e.MoveNext() )
                        b = (Box)e.Current;
                    else
                        b = null;
                }
                PendingWidth = prevLineArea.GetPendingWidth() - eatenWidth;
            }
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
            PendingAreas.Add( pia );
            PendingWidth += width;
            Prev = Text;

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
                var isMultiByteChar = false;
                var isText = false;
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

                    if ( Prev == Whitespace )
                    {
                        if ( _whiteSpaceCollapse == GenericBoolean.Enums.False )
                        {
                            if ( IsSpace( c ) )
                                SpaceWidth += GetCharWidth( c );
                            else if ( c == '\n' || c == '\u2028' )
                            {
                                if ( SpaceWidth > 0 )
                                {
                                    var isp = new InlineSpace( SpaceWidth );
                                    isp.SetUnderlined( textState.GetUnderlined() );
                                    isp.SetOverlined( textState.GetOverlined() );
                                    isp.SetLineThrough( textState.GetLineThrough() );
                                    AddChild( isp );
                                    FinalWidth += SpaceWidth;
                                    SpaceWidth = 0;
                                }
                                return i + 1;
                            }
                            else if ( c == '\t' )
                                SpaceWidth += 8 * whitespaceWidth;
                        }
                        else if ( c == '\u2028' )
                        {
                            if ( SpaceWidth > 0 )
                            {
                                var isp = new InlineSpace( SpaceWidth );
                                isp.SetUnderlined( textState.GetUnderlined() );
                                isp.SetOverlined( textState.GetOverlined() );
                                isp.SetLineThrough( textState.GetLineThrough() );
                                AddChild( isp );
                                FinalWidth += SpaceWidth;
                                SpaceWidth = 0;
                            }
                            return i + 1;
                        }
                    }
                    else if ( Prev == Text || Prev == Multibytechar )
                    {
                        if ( SpaceWidth > 0 )
                        {
                            var isp = new InlineSpace( SpaceWidth );
                            if ( PrevUlState )
                                isp.SetUnderlined( textState.GetUnderlined() );
                            if ( PrevOlState )
                                isp.SetOverlined( textState.GetOverlined() );
                            if ( PrevLtState )
                                isp.SetLineThrough( textState.GetLineThrough() );
                            AddChild( isp );
                            FinalWidth += SpaceWidth;
                            SpaceWidth = 0;
                        }

                        IEnumerator e = PendingAreas.GetEnumerator();
                        while ( e.MoveNext() )
                        {
                            var box = (Box)e.Current;
                            if ( box is InlineArea )
                            {
                                if ( ls != null )
                                {
                                    var lr =
                                        new Rectangle( FinalWidth, 0,
                                            ( (InlineArea)box ).GetContentWidth(),
                                            FontState.FontSize );
                                    ls.AddRect( lr, this, (InlineArea)box );
                                }
                            }
                            AddChild( box );
                        }

                        FinalWidth += PendingWidth;

                        PendingWidth = 0;
                        PendingAreas = new ArrayList();

                        if ( wordLength > 0 )
                        {
                            AddSpacedWord( new string( data, wordStart, wordLength ),
                                ls, FinalWidth, 0, textState, false );
                            FinalWidth += wordWidth;

                            wordWidth = 0;
                        }

                        Prev = Whitespace;

                        EmbeddedLinkStart = 0;

                        SpaceWidth = GetCharWidth( c );

                        if ( _whiteSpaceCollapse == GenericBoolean.Enums.False )
                        {
                            if ( c == '\n' || c == '\u2028' )
                                return i + 1;
                            if ( c == '\t' )
                                SpaceWidth = whitespaceWidth;
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
                                Prev = Whitespace;
                                SpaceWidth = GetCharWidth( c );
                            }
                            else if ( c == '\n' )
                            {
                                var isp = new InlineSpace( SpaceWidth );
                                AddChild( isp );
                                return i + 1;
                            }
                            else if ( c == '\t' )
                            {
                                Prev = Whitespace;
                                SpaceWidth = 8 * whitespaceWidth;
                            }
                        }
                        else
                            wordStart++;
                    }
                }

                if ( isText )
                {
                    int curr = isMultiByteChar ? Multibytechar : Text;
                    if ( Prev == Whitespace )
                    {
                        wordWidth = charWidth;
                        if ( FinalWidth + SpaceWidth + wordWidth
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
                        Prev = curr;
                        wordStart = i;
                        wordLength = 1;
                    }
                    else if ( Prev == Text || Prev == Multibytechar )
                    {
                        if ( Prev == Text && curr == Text || !CanBreakMidWord() )
                        {
                            wordLength++;
                            wordWidth += charWidth;
                        }
                        else
                        {
                            var isp = new InlineSpace( SpaceWidth );
                            if ( PrevUlState )
                                isp.SetUnderlined( textState.GetUnderlined() );
                            if ( PrevOlState )
                                isp.SetOverlined( textState.GetOverlined() );
                            if ( PrevLtState )
                                isp.SetLineThrough( textState.GetLineThrough() );
                            AddChild( isp );
                            FinalWidth += SpaceWidth;
                            SpaceWidth = 0;

                            IEnumerator e = PendingAreas.GetEnumerator();
                            while ( e.MoveNext() )
                            {
                                var box = (Box)e.Current;
                                if ( box is InlineArea )
                                {
                                    if ( ls != null )
                                    {
                                        var lr =
                                            new Rectangle( FinalWidth, 0,
                                                ( (InlineArea)box ).GetContentWidth(),
                                                FontState.FontSize );
                                        ls.AddRect( lr, this, (InlineArea)box );
                                    }
                                }
                                AddChild( box );
                            }

                            FinalWidth += PendingWidth;

                            PendingWidth = 0;
                            PendingAreas = new ArrayList();

                            if ( wordLength > 0 )
                            {
                                AddSpacedWord( new string( data, wordStart, wordLength ),
                                    ls, FinalWidth, 0, textState, false );
                                FinalWidth += wordWidth;
                            }
                            SpaceWidth = 0;
                            wordStart = i;
                            wordLength = 1;
                            wordWidth = charWidth;
                        }
                        Prev = curr;
                    }
                    else
                    {
                        Prev = curr;
                        wordStart = i;
                        wordLength = 1;
                        wordWidth = charWidth;
                    }

                    if ( FinalWidth + SpaceWidth + PendingWidth + wordWidth
                        > GetContentWidth() )
                    {
                        if ( _wrapOption == WrapOption.Wrap )
                        {
                            if ( wordStart == start )
                            {
                                overrun = true;
                                if ( FinalWidth > 0 )
                                    return wordStart;
                            }
                            else
                                return wordStart;
                        }
                    }
                }
            }

            if ( Prev == Text || Prev == Multibytechar )
            {
                if ( SpaceWidth > 0 )
                {
                    var pis = new InlineSpace( SpaceWidth );
                    pis.SetEatable( true );
                    if ( PrevUlState )
                        pis.SetUnderlined( textState.GetUnderlined() );
                    if ( PrevOlState )
                        pis.SetOverlined( textState.GetOverlined() );
                    if ( PrevLtState )
                        pis.SetLineThrough( textState.GetLineThrough() );
                    PendingAreas.Add( pis );
                    PendingWidth += SpaceWidth;
                    SpaceWidth = 0;
                }

                AddSpacedWord( new string( data, wordStart, wordLength ), ls,
                    FinalWidth + PendingWidth,
                    SpaceWidth, textState, true );

                EmbeddedLinkStart += wordWidth;
                wordWidth = 0;
            }

            if ( overrun )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Area contents overflows area" );
            }
            return -1;
        }

        public void AddLeader( int leaderPattern, int leaderLengthMinimum,
            int leaderLengthOptimum, int leaderLengthMaximum,
            int ruleStyle, int ruleThickness,
            int leaderPatternWidth, int leaderAlignment )
        {
            var leaderLength = 0;
            const char dotIndex = '.';
            int dotWidth = _currentFontState.GetWidth( _currentFontState.MapCharacter( dotIndex ) );
            const char whitespaceIndex = ' ';
            int whitespaceWidth =
                _currentFontState.GetWidth( _currentFontState.MapCharacter( whitespaceIndex ) );

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
                PendingAreas.Add( spaceArea );
                break;

            case LeaderPattern.Rule:
                var leaderArea = new LeaderArea( FontState, _red, _green, _blue,
                    string.Empty, leaderLength, leaderPattern, ruleThickness, ruleStyle );
                leaderArea.SetYOffset( _placementOffset );
                PendingAreas.Add( leaderArea );
                break;

            case LeaderPattern.Dots:
                if ( leaderPatternWidth < dotWidth ) leaderPatternWidth = 0;
                if ( leaderPatternWidth == 0 )
                {
                    PendingAreas.Add( BuildSimpleLeader( dotIndex, leaderLength ) );
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
                            PendingAreas.Add( new InlineSpace( spaceBeforeLeader,
                                false ) );
                            PendingWidth += spaceBeforeLeader;
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
                        PendingAreas.Add( leaderPatternArea );
                        PendingAreas.Add( spaceBetweenDots );
                    }

                    PendingAreas.Add( new InlineSpace( leaderLength - dotsFactor * leaderPatternWidth ) );
                }
                break;

            case LeaderPattern.Usecontent:
                FonetDriver.ActiveDriver.FireFonetError(
                    "leader-pattern=\"use-content\" not supported by this version of FO.NET" );
                return;
            }

            PendingWidth += leaderLength;
            Prev = Text;
        }

        public void AddPending()
        {
            if ( SpaceWidth > 0 )
            {
                AddChild( new InlineSpace( SpaceWidth ) );
                FinalWidth += SpaceWidth;
                SpaceWidth = 0;
            }

            foreach ( Box box in PendingAreas )
                AddChild( box );

            FinalWidth += PendingWidth;

            PendingWidth = 0;
            PendingAreas = new ArrayList();
        }

        public void Align( int type )
        {
            int padding;

            switch ( type )
            {
            case TextAlign.Start:
                padding = GetContentWidth() - FinalWidth;
                EndIndent += padding;
                break;
            case TextAlign.End:
                padding = GetContentWidth() - FinalWidth;
                StartIndent += padding;
                break;
            case TextAlign.Center:
                padding = ( GetContentWidth() - FinalWidth ) / 2;
                StartIndent += padding;
                EndIndent += padding;
                break;
            case TextAlign.Justify:
                var spaceCount = 0;
                foreach ( Box b in Children )
                {
                    if ( b is InlineSpace )
                    {
                        var space = (InlineSpace)b;
                        if ( space.GetResizeable() )
                            spaceCount++;
                    }
                }
                if ( spaceCount > 0 )
                    padding = ( GetContentWidth() - FinalWidth ) / spaceCount;
                else
                    padding = 0;
                spaceCount = 0;
                foreach ( Box b in Children )
                {
                    if ( b is InlineSpace )
                    {
                        var space = (InlineSpace)b;
                        if ( space.GetResizeable() )
                        {
                            space.SetSize( space.GetSize() + padding );
                            spaceCount++;
                        }
                    }
                    else if ( b is InlineArea )
                        ( (InlineArea)b ).SetXOffset( spaceCount * padding );
                }
                break;
            }
        }

        public void VerticalAlign()
        {
            int superHeight = -_placementOffset;
            int maxHeight = AllocationHeight;
            foreach ( Box b in Children )
            {
                if ( b is InlineArea )
                {
                    var ia = (InlineArea)b;
                    if ( ia is WordArea )
                        ia.SetYOffset( _placementOffset );
                    if ( ia.GetHeight() > maxHeight )
                        maxHeight = ia.GetHeight();
                    int vert = ia.GetVerticalAlign();
                    if ( vert == Fo.Properties.VerticalAlign.Super )
                    {
                        int fh = FontState.Ascender;
                        ia.SetYOffset( (int)( _placementOffset - 2 * fh / 3.0 ) );
                    }
                    else if ( vert == Fo.Properties.VerticalAlign.Sub )
                    {
                        int fh = FontState.Ascender;
                        ia.SetYOffset( (int)( _placementOffset + 2 * fh / 3.0 ) );
                    }
                }
            }
            AllocationHeight = maxHeight;
        }

        public void ChangeColor( float red, float green, float blue )
        {
            this._red = red;
            this._green = green;
            this._blue = blue;
        }

        public void ChangeFont( FontState fontState )
        {
            _currentFontState = fontState;
        }

        public void ChangeWhiteSpaceCollapse( int whiteSpaceCollapse )
        {
            this._whiteSpaceCollapse = whiteSpaceCollapse;
        }

        public void ChangeWrapOption( int wrapOption )
        {
            this._wrapOption = wrapOption;
        }

        public void ChangeVerticalAlign( int vAlign )
        {
            this._vAlign = vAlign;
        }

        public int GetEndIndent()
        {
            return EndIndent;
        }

        public override int GetHeight()
        {
            return AllocationHeight;
        }

        public int GetPlacementOffset()
        {
            return _placementOffset;
        }

        public int GetStartIndent()
        {
            return StartIndent;
        }

        public bool IsEmpty()
        {
            return !( PendingAreas.Count > 0 || Children.Count > 0 );
        }

        public ArrayList GetPendingAreas()
        {
            return PendingAreas;
        }

        public int GetPendingWidth()
        {
            return PendingWidth;
        }

        public void SetPendingAreas( ArrayList areas )
        {
            PendingAreas = areas;
        }

        public void SetPendingWidth( int width )
        {
            PendingWidth = width;
        }

        public void ChangeHyphenation( HyphenationProps hyphProps )
        {
            this._hyphProps = hyphProps;
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

        private int GetLeaderAlignIndent( int leaderLength,
            int leaderPatternWidth )
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
            return FinalWidth + SpaceWidth + StartIndent + PendingWidth;
        }

        private string GetHyphenationWord( char[] characters, int wordStart )
        {
            var wordendFound = false;
            var counter = 0;
            var newWord = new char[ characters.Length ];
            while ( !wordendFound
                && wordStart + counter < characters.Length )
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
            if ( word == null )
                return 0;
            var width = 0;
            foreach ( char c in word )
                width += GetCharWidth( c );
            return width;
        }

        public int GetRemainingWidth()
        {
            return GetContentWidth() + StartIndent - GetCurrentXPosition();
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
                var lr = new Rectangle( FinalWidth, 0, box.GetContentWidth(), box.GetContentHeight() );
                ls.AddRect( lr, this, box );
            }
            Prev = Text;
            FinalWidth += box.GetContentWidth();
        }

        public void AddInlineSpace( InlineSpace isp, int spaceWidth )
        {
            AddChild( isp );
            FinalWidth += spaceWidth;
        }

        public int AddCharacter( char data, LinkSet ls, bool ul )
        {
            WordArea ia = null;
            int remainingWidth = GetRemainingWidth();
            int width =
                _currentFontState.GetWidth( _currentFontState.MapCharacter( data ) );
            if ( width > remainingWidth )
                return Character.DoesnotFit;
            if ( char.IsWhiteSpace( data )
                && _whiteSpaceCollapse == GenericBoolean.Enums.True )
                return Character.Ok;
            ia = new WordArea( _currentFontState, _red, _green,
                _blue, data.ToString(),
                width );
            ia.SetYOffset( _placementOffset );
            ia.SetUnderlined( ul );
            PendingAreas.Add( ia );
            if ( char.IsWhiteSpace( data ) )
            {
                SpaceWidth = +width;
                Prev = Whitespace;
            }
            else
            {
                PendingWidth += width;
                Prev = Text;
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

            if ( IsAnySpace( startChar ) )
                AddChild( new InlineSpace( startCharWidth ) );
            else
            {
                hia = new WordArea( _currentFontState, _red, _green,
                    _blue,
                    startChar.ToString(), 1 );
                hia.SetYOffset( _placementOffset );
                AddChild( hia );
            }
            int wordWidth = GetWordWidth( word );
            hia = new WordArea( _currentFontState, _red, _green, _blue,
                word, word.Length );
            hia.SetYOffset( _placementOffset );
            AddChild( hia );

            FinalWidth += startCharWidth + wordWidth;
        }

        private bool CanBreakMidWord()
        {
            var ret = false;
            if ( _hyphProps != null && _hyphProps.Language != null
                && !_hyphProps.Language.Equals( "NONE" ) )
            {
                string lang = _hyphProps.Language.ToLower();
                if ( "zh".Equals( lang ) || "ja".Equals( lang ) || "ko".Equals( lang )
                    || "vi".Equals( lang ) )
                    ret = true;
            }
            return ret;
        }

        private int GetCharWidth( char c )
        {
            int width;

            if ( c == '\n' || c == '\r' || c == '\t' || c == '\u00A0' )
                width = GetCharWidth( ' ' );
            else
            {
                width = _currentFontState.GetWidth( _currentFontState.MapCharacter( c ) );
                if ( width <= 0 )
                {
                    int em = _currentFontState.GetWidth( _currentFontState.MapCharacter( 'm' ) );
                    int en = _currentFontState.GetWidth( _currentFontState.MapCharacter( 'n' ) );
                    if ( em <= 0 )
                        em = 500 * _currentFontState.FontSize;
                    if ( en <= 0 )
                        en = em - 10;

                    if ( c == ' ' )
                        width = em;
                    if ( c == '\u2000' )
                        width = en;
                    if ( c == '\u2001' )
                        width = em;
                    if ( c == '\u2002' )
                        width = em / 2;
                    if ( c == '\u2003' )
                        width = _currentFontState.FontSize;
                    if ( c == '\u2004' )
                        width = em / 3;
                    if ( c == '\u2005' )
                        width = em / 4;
                    if ( c == '\u2006' )
                        width = em / 6;
                    if ( c == '\u2007' )
                        width = GetCharWidth( ' ' );
                    if ( c == '\u2008' )
                        width = GetCharWidth( '.' );
                    if ( c == '\u2009' )
                        width = em / 5;
                    if ( c == '\u200A' )
                        width = 5;
                    if ( c == '\u200B' )
                        width = 100;
                    if ( c == '\u202F' )
                        width = GetCharWidth( ' ' ) / 2;
                    if ( c == '\u3000' )
                        width = GetCharWidth( ' ' ) * 2;
                }
            }

            return width;
        }

        private bool IsSpace( char c )
        {
            if ( c == ' ' || c == '\u2000' || // en quad
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
                c == '\u200B' ) // zero width space
                return true;
            return false;
        }

        private bool IsNbsp( char c )
        {
            if ( c == '\u00A0' || c == '\u202F' || // narrow no-break space
                c == '\u3000' || // ideographic space
                c == '\uFEFF' )
            {
                // zero width no-break space
                return true;
            }
            return false;
        }

        private bool IsAnySpace( char c )
        {
            bool ret = IsSpace( c ) || IsNbsp( c );
            return ret;
        }

        private void AddSpacedWord( string word, LinkSet ls, int startw,
            int spacew, TextState textState,
            bool addToPending )
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
                    if ( spaceWidth > 0 )
                    {
                        var ispace = new InlineSpace( spaceWidth );
                        extraw += spaceWidth;
                        if ( PrevUlState )
                            ispace.SetUnderlined( textState.GetUnderlined() );
                        if ( PrevOlState )
                            ispace.SetOverlined( textState.GetOverlined() );
                        if ( PrevLtState )
                            ispace.SetLineThrough( textState.GetLineThrough() );

                        if ( addToPending )
                        {
                            PendingAreas.Add( ispace );
                            PendingWidth += spaceWidth;
                        }
                        else
                            AddChild( ispace );
                    }
                }
                else
                {
                    var ia = new WordArea( _currentFontState, _red,
                        _green, _blue,
                        currentWord,
                        GetWordWidth( currentWord ) );
                    ia.SetYOffset( _placementOffset );
                    ia.SetUnderlined( textState.GetUnderlined() );
                    PrevUlState = textState.GetUnderlined();
                    ia.SetOverlined( textState.GetOverlined() );
                    PrevOlState = textState.GetOverlined();
                    ia.SetLineThrough( textState.GetLineThrough() );
                    PrevLtState = textState.GetLineThrough();
                    ia.SetVerticalAlign( _vAlign );

                    if ( addToPending )
                    {
                        PendingAreas.Add( ia );
                        PendingWidth += GetWordWidth( currentWord );
                    }
                    else
                        AddChild( ia );
                    if ( ls != null )
                    {
                        var lr = new Rectangle( startw + extraw, spacew,
                            ia.GetContentWidth(),
                            FontState.FontSize );
                        ls.AddRect( lr, this, ia );
                    }
                }
            }
        }
    }
}