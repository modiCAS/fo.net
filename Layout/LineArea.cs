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
        protected const int NOTHING = 0;
        protected const int WHITESPACE = 1;
        protected const int TEXT = 2;
        protected const int MULTIBYTECHAR = 3;
        protected int allocationHeight;
        private FontState currentFontState;
        protected int embeddedLinkStart;
        protected int endIndent;
        protected int finalWidth;
        protected int halfLeading;
        private HyphenationProps hyphProps;
        protected int lineHeight;
        protected int nominalFontSize;
        protected int nominalGlyphHeight;
        protected ArrayList pendingAreas = new ArrayList();
        protected int pendingWidth;
        private readonly int placementOffset;
        protected int prev = NOTHING;
        protected bool prevLTState;
        protected bool prevOlState;
        protected bool prevUlState;
        private float red, green, blue;
        protected int spaceWidth;
        protected int startIndent;
        private int vAlign;
        private int whiteSpaceCollapse;
        private int wrapOption;

        public LineArea( FontState fontState, int lineHeight, int halfLeading,
            int allocationWidth, int startIndent, int endIndent,
            LineArea prevLineArea )
            : base( fontState )
        {
            currentFontState = fontState;
            this.lineHeight = lineHeight;
            nominalFontSize = fontState.FontSize;
            nominalGlyphHeight = fontState.Ascender - fontState.Descender;

            placementOffset = fontState.Ascender;
            contentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            this.fontState = fontState;

            allocationHeight = nominalGlyphHeight;
            this.halfLeading = this.lineHeight - allocationHeight;

            this.startIndent = startIndent;
            this.endIndent = endIndent;

            if ( prevLineArea != null )
            {
                IEnumerator e = prevLineArea.pendingAreas.GetEnumerator();
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
                            if ( isp.isEatable() )
                                eatenWidth += isp.getSize();
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
                    pendingAreas.Add( b );
                    if ( e.MoveNext() )
                        b = (Box)e.Current;
                    else
                        b = null;
                }
                pendingWidth = prevLineArea.getPendingWidth() - eatenWidth;
            }
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderLineArea( this );
        }

        public int addPageNumberCitation( string refid, LinkSet ls )
        {
            int width = currentFontState.GetWidth( currentFontState.MapCharacter( ' ' ) );


            var pia = new PageNumberInlineArea( currentFontState,
                red, green, blue, refid, width );

            pia.setYOffset( placementOffset );
            pendingAreas.Add( pia );
            pendingWidth += width;
            prev = TEXT;

            return -1;
        }

        public int addText( char[] odata, int start, int end, LinkSet ls,
            TextState textState )
        {
            if ( start == -1 )
                return -1;
            var overrun = false;

            int wordStart = start;
            var wordLength = 0;
            var wordWidth = 0;
            int whitespaceWidth = getCharWidth( ' ' );

            var data = new char[ odata.Length ];
            var dataCopy = new char[ odata.Length ];
            Array.Copy( odata, data, odata.Length );
            Array.Copy( odata, dataCopy, odata.Length );

            var isText = false;
            var isMultiByteChar = false;

            for ( int i = start; i < end; i++ )
            {
                int charWidth;
                char c = data[ i ];
                if ( !( isSpace( c ) || c == '\n' || c == '\r' || c == '\t'
                    || c == '\u2028' ) )
                {
                    charWidth = getCharWidth( c );
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
                        charWidth = getCharWidth( c );

                    isText = false;
                    isMultiByteChar = false;

                    if ( prev == WHITESPACE )
                    {
                        if ( whiteSpaceCollapse == GenericBoolean.Enums.FALSE )
                        {
                            if ( isSpace( c ) )
                                spaceWidth += getCharWidth( c );
                            else if ( c == '\n' || c == '\u2028' )
                            {
                                if ( spaceWidth > 0 )
                                {
                                    var isp = new InlineSpace( spaceWidth );
                                    isp.setUnderlined( textState.getUnderlined() );
                                    isp.setOverlined( textState.getOverlined() );
                                    isp.setLineThrough( textState.getLineThrough() );
                                    addChild( isp );
                                    finalWidth += spaceWidth;
                                    spaceWidth = 0;
                                }
                                return i + 1;
                            }
                            else if ( c == '\t' )
                                spaceWidth += 8 * whitespaceWidth;
                        }
                        else if ( c == '\u2028' )
                        {
                            if ( spaceWidth > 0 )
                            {
                                var isp = new InlineSpace( spaceWidth );
                                isp.setUnderlined( textState.getUnderlined() );
                                isp.setOverlined( textState.getOverlined() );
                                isp.setLineThrough( textState.getLineThrough() );
                                addChild( isp );
                                finalWidth += spaceWidth;
                                spaceWidth = 0;
                            }
                            return i + 1;
                        }
                    }
                    else if ( prev == TEXT || prev == MULTIBYTECHAR )
                    {
                        if ( spaceWidth > 0 )
                        {
                            var isp = new InlineSpace( spaceWidth );
                            if ( prevUlState )
                                isp.setUnderlined( textState.getUnderlined() );
                            if ( prevOlState )
                                isp.setOverlined( textState.getOverlined() );
                            if ( prevLTState )
                                isp.setLineThrough( textState.getLineThrough() );
                            addChild( isp );
                            finalWidth += spaceWidth;
                            spaceWidth = 0;
                        }

                        IEnumerator e = pendingAreas.GetEnumerator();
                        while ( e.MoveNext() )
                        {
                            var box = (Box)e.Current;
                            if ( box is InlineArea )
                            {
                                if ( ls != null )
                                {
                                    var lr =
                                        new Rectangle( finalWidth, 0,
                                            ( (InlineArea)box ).getContentWidth(),
                                            fontState.FontSize );
                                    ls.addRect( lr, this, (InlineArea)box );
                                }
                            }
                            addChild( box );
                        }

                        finalWidth += pendingWidth;

                        pendingWidth = 0;
                        pendingAreas = new ArrayList();

                        if ( wordLength > 0 )
                        {
                            addSpacedWord( new string( data, wordStart, wordLength ),
                                ls, finalWidth, 0, textState, false );
                            finalWidth += wordWidth;

                            wordWidth = 0;
                        }

                        prev = WHITESPACE;

                        embeddedLinkStart = 0;

                        spaceWidth = getCharWidth( c );

                        if ( whiteSpaceCollapse == GenericBoolean.Enums.FALSE )
                        {
                            if ( c == '\n' || c == '\u2028' )
                                return i + 1;
                            if ( c == '\t' )
                                spaceWidth = whitespaceWidth;
                        }
                        else if ( c == '\u2028' )
                            return i + 1;
                    }
                    else
                    {
                        if ( whiteSpaceCollapse == GenericBoolean.Enums.FALSE )
                        {
                            if ( isSpace( c ) )
                            {
                                prev = WHITESPACE;
                                spaceWidth = getCharWidth( c );
                            }
                            else if ( c == '\n' )
                            {
                                var isp = new InlineSpace( spaceWidth );
                                addChild( isp );
                                return i + 1;
                            }
                            else if ( c == '\t' )
                            {
                                prev = WHITESPACE;
                                spaceWidth = 8 * whitespaceWidth;
                            }
                        }
                        else
                            wordStart++;
                    }
                }

                if ( isText )
                {
                    int curr = isMultiByteChar ? MULTIBYTECHAR : TEXT;
                    if ( prev == WHITESPACE )
                    {
                        wordWidth = charWidth;
                        if ( finalWidth + spaceWidth + wordWidth
                            > getContentWidth() )
                        {
                            if ( overrun )
                            {
                                FonetDriver.ActiveDriver.FireFonetWarning(
                                    "Area contents overflows area" );
                            }
                            if ( wrapOption == WrapOption.WRAP )
                                return i;
                        }
                        prev = curr;
                        wordStart = i;
                        wordLength = 1;
                    }
                    else if ( prev == TEXT || prev == MULTIBYTECHAR )
                    {
                        if ( prev == TEXT && curr == TEXT || !canBreakMidWord() )
                        {
                            wordLength++;
                            wordWidth += charWidth;
                        }
                        else
                        {
                            var isp = new InlineSpace( spaceWidth );
                            if ( prevUlState )
                                isp.setUnderlined( textState.getUnderlined() );
                            if ( prevOlState )
                                isp.setOverlined( textState.getOverlined() );
                            if ( prevLTState )
                                isp.setLineThrough( textState.getLineThrough() );
                            addChild( isp );
                            finalWidth += spaceWidth;
                            spaceWidth = 0;

                            IEnumerator e = pendingAreas.GetEnumerator();
                            while ( e.MoveNext() )
                            {
                                var box = (Box)e.Current;
                                if ( box is InlineArea )
                                {
                                    if ( ls != null )
                                    {
                                        var lr =
                                            new Rectangle( finalWidth, 0,
                                                ( (InlineArea)box ).getContentWidth(),
                                                fontState.FontSize );
                                        ls.addRect( lr, this, (InlineArea)box );
                                    }
                                }
                                addChild( box );
                            }

                            finalWidth += pendingWidth;

                            pendingWidth = 0;
                            pendingAreas = new ArrayList();

                            if ( wordLength > 0 )
                            {
                                addSpacedWord( new string( data, wordStart, wordLength ),
                                    ls, finalWidth, 0, textState, false );
                                finalWidth += wordWidth;
                            }
                            spaceWidth = 0;
                            wordStart = i;
                            wordLength = 1;
                            wordWidth = charWidth;
                        }
                        prev = curr;
                    }
                    else
                    {
                        prev = curr;
                        wordStart = i;
                        wordLength = 1;
                        wordWidth = charWidth;
                    }

                    if ( finalWidth + spaceWidth + pendingWidth + wordWidth
                        > getContentWidth() )
                    {
                        if ( wrapOption == WrapOption.WRAP )
                        {
                            if ( wordStart == start )
                            {
                                overrun = true;
                                if ( finalWidth > 0 )
                                    return wordStart;
                            }
                            else
                                return wordStart;
                        }
                    }
                }
            }

            if ( prev == TEXT || prev == MULTIBYTECHAR )
            {
                if ( spaceWidth > 0 )
                {
                    var pis = new InlineSpace( spaceWidth );
                    pis.setEatable( true );
                    if ( prevUlState )
                        pis.setUnderlined( textState.getUnderlined() );
                    if ( prevOlState )
                        pis.setOverlined( textState.getOverlined() );
                    if ( prevLTState )
                        pis.setLineThrough( textState.getLineThrough() );
                    pendingAreas.Add( pis );
                    pendingWidth += spaceWidth;
                    spaceWidth = 0;
                }

                addSpacedWord( new string( data, wordStart, wordLength ), ls,
                    finalWidth + pendingWidth,
                    spaceWidth, textState, true );

                embeddedLinkStart += wordWidth;
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
            WordArea leaderPatternArea;
            var leaderLength = 0;
            var dotIndex = '.';
            int dotWidth =
                currentFontState.GetWidth( currentFontState.MapCharacter( dotIndex ) );
            var whitespaceIndex = ' ';
            int whitespaceWidth =
                currentFontState.GetWidth( currentFontState.MapCharacter( whitespaceIndex ) );

            int remainingWidth = getRemainingWidth();

            if ( remainingWidth <= leaderLengthOptimum
                || remainingWidth <= leaderLengthMaximum )
                leaderLength = remainingWidth;
            else if ( remainingWidth > leaderLengthOptimum
                && remainingWidth > leaderLengthMaximum )
                leaderLength = leaderLengthMaximum;
            else if ( leaderLengthOptimum > leaderLengthMaximum
                && leaderLengthOptimum < remainingWidth )
                leaderLength = leaderLengthOptimum;

            if ( leaderLength <= 0 )
                return;

            switch ( leaderPattern )
            {
            case LeaderPattern.SPACE:
                var spaceArea = new InlineSpace( leaderLength );
                pendingAreas.Add( spaceArea );
                break;
            case LeaderPattern.RULE:
                var leaderArea = new LeaderArea( fontState, red, green,
                    blue, "", leaderLength,
                    leaderPattern,
                    ruleThickness, ruleStyle );
                leaderArea.setYOffset( placementOffset );
                pendingAreas.Add( leaderArea );
                break;
            case LeaderPattern.DOTS:
                if ( leaderPatternWidth < dotWidth )
                    leaderPatternWidth = 0;
                if ( leaderPatternWidth == 0 )
                {
                    pendingAreas.Add( buildSimpleLeader( dotIndex,
                        leaderLength ) );
                }
                else
                {
                    if ( leaderAlignment == LeaderAlignment.REFERENCE_AREA )
                    {
                        int spaceBeforeLeader =
                            getLeaderAlignIndent( leaderLength,
                                leaderPatternWidth );
                        if ( spaceBeforeLeader != 0 )
                        {
                            pendingAreas.Add( new InlineSpace( spaceBeforeLeader,
                                false ) );
                            pendingWidth += spaceBeforeLeader;
                            leaderLength -= spaceBeforeLeader;
                        }
                    }

                    var spaceBetweenDots =
                        new InlineSpace( leaderPatternWidth - dotWidth, false );

                    leaderPatternArea = new WordArea( currentFontState, red,
                        green, blue,
                        ".", dotWidth );
                    leaderPatternArea.setYOffset( placementOffset );
                    var dotsFactor =
                        (int)Math.Floor( leaderLength
                            / (double)leaderPatternWidth );

                    for ( var i = 0; i < dotsFactor; i++ )
                    {
                        pendingAreas.Add( leaderPatternArea );
                        pendingAreas.Add( spaceBetweenDots );
                    }
                    pendingAreas.Add( new InlineSpace( leaderLength
                        - dotsFactor
                            * leaderPatternWidth ) );
                }
                break;
            case LeaderPattern.USECONTENT:
                FonetDriver.ActiveDriver.FireFonetError(
                    "leader-pattern=\"use-content\" not supported by this version of FO.NET" );
                return;
            }
            pendingWidth += leaderLength;
            prev = TEXT;
        }

        public void addPending()
        {
            if ( spaceWidth > 0 )
            {
                addChild( new InlineSpace( spaceWidth ) );
                finalWidth += spaceWidth;
                spaceWidth = 0;
            }

            foreach ( Box box in pendingAreas )
                addChild( box );

            finalWidth += pendingWidth;

            pendingWidth = 0;
            pendingAreas = new ArrayList();
        }

        public void align( int type )
        {
            var padding = 0;

            switch ( type )
            {
            case TextAlign.START:
                padding = getContentWidth() - finalWidth;
                endIndent += padding;
                break;
            case TextAlign.END:
                padding = getContentWidth() - finalWidth;
                startIndent += padding;
                break;
            case TextAlign.CENTER:
                padding = ( getContentWidth() - finalWidth ) / 2;
                startIndent += padding;
                endIndent += padding;
                break;
            case TextAlign.JUSTIFY:
                var spaceCount = 0;
                foreach ( Box b in children )
                {
                    if ( b is InlineSpace )
                    {
                        var space = (InlineSpace)b;
                        if ( space.getResizeable() )
                            spaceCount++;
                    }
                }
                if ( spaceCount > 0 )
                    padding = ( getContentWidth() - finalWidth ) / spaceCount;
                else
                    padding = 0;
                spaceCount = 0;
                foreach ( Box b in children )
                {
                    if ( b is InlineSpace )
                    {
                        var space = (InlineSpace)b;
                        if ( space.getResizeable() )
                        {
                            space.setSize( space.getSize() + padding );
                            spaceCount++;
                        }
                    }
                    else if ( b is InlineArea )
                        ( (InlineArea)b ).setXOffset( spaceCount * padding );
                }
                break;
            }
        }

        public void verticalAlign()
        {
            int superHeight = -placementOffset;
            int maxHeight = allocationHeight;
            foreach ( Box b in children )
            {
                if ( b is InlineArea )
                {
                    var ia = (InlineArea)b;
                    if ( ia is WordArea )
                        ia.setYOffset( placementOffset );
                    if ( ia.GetHeight() > maxHeight )
                        maxHeight = ia.GetHeight();
                    int vert = ia.getVerticalAlign();
                    if ( vert == VerticalAlign.SUPER )
                    {
                        int fh = fontState.Ascender;
                        ia.setYOffset( (int)( placementOffset - 2 * fh / 3.0 ) );
                    }
                    else if ( vert == VerticalAlign.SUB )
                    {
                        int fh = fontState.Ascender;
                        ia.setYOffset( (int)( placementOffset + 2 * fh / 3.0 ) );
                    }
                }
            }
            allocationHeight = maxHeight;
        }

        public void changeColor( float red, float green, float blue )
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public void changeFont( FontState fontState )
        {
            currentFontState = fontState;
        }

        public void changeWhiteSpaceCollapse( int whiteSpaceCollapse )
        {
            this.whiteSpaceCollapse = whiteSpaceCollapse;
        }

        public void changeWrapOption( int wrapOption )
        {
            this.wrapOption = wrapOption;
        }

        public void changeVerticalAlign( int vAlign )
        {
            this.vAlign = vAlign;
        }

        public int getEndIndent()
        {
            return endIndent;
        }

        public override int GetHeight()
        {
            return allocationHeight;
        }

        public int getPlacementOffset()
        {
            return placementOffset;
        }

        public int getStartIndent()
        {
            return startIndent;
        }

        public bool isEmpty()
        {
            return !( pendingAreas.Count > 0 || children.Count > 0 );
        }

        public ArrayList getPendingAreas()
        {
            return pendingAreas;
        }

        public int getPendingWidth()
        {
            return pendingWidth;
        }

        public void setPendingAreas( ArrayList areas )
        {
            pendingAreas = areas;
        }

        public void setPendingWidth( int width )
        {
            pendingWidth = width;
        }

        public void changeHyphenation( HyphenationProps hyphProps )
        {
            this.hyphProps = hyphProps;
        }

        private InlineArea buildSimpleLeader( char c, int leaderLength )
        {
            int width = currentFontState.GetWidth( currentFontState.MapCharacter( c ) );
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
            var leaderPatternArea = new WordArea( currentFontState, red,
                green, blue,
                new string( leaderChars ),
                leaderLength );
            leaderPatternArea.setYOffset( placementOffset );
            return leaderPatternArea;
        }

        private int getLeaderAlignIndent( int leaderLength,
            int leaderPatternWidth )
        {
            double position = getCurrentXPosition();
            double nextRepeatedLeaderPatternCycle = Math.Ceiling( position
                / leaderPatternWidth );
            double difference =
                leaderPatternWidth * nextRepeatedLeaderPatternCycle - position;
            return (int)difference;
        }

        private int getCurrentXPosition()
        {
            return finalWidth + spaceWidth + startIndent + pendingWidth;
        }

        private string getHyphenationWord( char[] characters, int wordStart )
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

        private int getWordWidth( string word )
        {
            if ( word == null )
                return 0;
            var width = 0;
            foreach ( char c in word )
                width += getCharWidth( c );
            return width;
        }

        public int getRemainingWidth()
        {
            return getContentWidth() + startIndent - getCurrentXPosition();
        }

        public void setLinkSet( LinkSet ls )
        {
        }

        public void addInlineArea( InlineArea box, LinkSet ls )
        {
            addPending();
            addChild( box );
            if ( ls != null )
            {
                var lr = new Rectangle( finalWidth, 0, box.getContentWidth(), box.getContentHeight() );
                ls.addRect( lr, this, box );
            }
            prev = TEXT;
            finalWidth += box.getContentWidth();
        }

        public void addInlineSpace( InlineSpace isp, int spaceWidth )
        {
            addChild( isp );
            finalWidth += spaceWidth;
        }

        public int addCharacter( char data, LinkSet ls, bool ul )
        {
            WordArea ia = null;
            int remainingWidth = getRemainingWidth();
            int width =
                currentFontState.GetWidth( currentFontState.MapCharacter( data ) );
            if ( width > remainingWidth )
                return Character.DOESNOT_FIT;
            if ( char.IsWhiteSpace( data )
                && whiteSpaceCollapse == GenericBoolean.Enums.TRUE )
                return Character.OK;
            ia = new WordArea( currentFontState, red, green,
                blue, data.ToString(),
                width );
            ia.setYOffset( placementOffset );
            ia.setUnderlined( ul );
            pendingAreas.Add( ia );
            if ( char.IsWhiteSpace( data ) )
            {
                spaceWidth = +width;
                prev = WHITESPACE;
            }
            else
            {
                pendingWidth += width;
                prev = TEXT;
            }
            return Character.OK;
        }

        private void addMapWord( char startChar, StringBuilder wordBuf )
        {
            var mapBuf = new StringBuilder( wordBuf.Length );
            for ( var i = 0; i < wordBuf.Length; i++ )
                mapBuf.Append( currentFontState.MapCharacter( wordBuf[ i ] ) );

            addWord( startChar, mapBuf );
        }

        private void addWord( char startChar, StringBuilder wordBuf )
        {
            string word = wordBuf != null ? wordBuf.ToString() : "";
            WordArea hia;
            int startCharWidth = getCharWidth( startChar );

            if ( isAnySpace( startChar ) )
                addChild( new InlineSpace( startCharWidth ) );
            else
            {
                hia = new WordArea( currentFontState, red, green,
                    blue,
                    startChar.ToString(), 1 );
                hia.setYOffset( placementOffset );
                addChild( hia );
            }
            int wordWidth = getWordWidth( word );
            hia = new WordArea( currentFontState, red, green, blue,
                word, word.Length );
            hia.setYOffset( placementOffset );
            addChild( hia );

            finalWidth += startCharWidth + wordWidth;
        }

        private bool canBreakMidWord()
        {
            var ret = false;
            if ( hyphProps != null && hyphProps.language != null
                && !hyphProps.language.Equals( "NONE" ) )
            {
                string lang = hyphProps.language.ToLower();
                if ( "zh".Equals( lang ) || "ja".Equals( lang ) || "ko".Equals( lang )
                    || "vi".Equals( lang ) )
                    ret = true;
            }
            return ret;
        }

        private int getCharWidth( char c )
        {
            int width;

            if ( c == '\n' || c == '\r' || c == '\t' || c == '\u00A0' )
                width = getCharWidth( ' ' );
            else
            {
                width = currentFontState.GetWidth( currentFontState.MapCharacter( c ) );
                if ( width <= 0 )
                {
                    int em = currentFontState.GetWidth( currentFontState.MapCharacter( 'm' ) );
                    int en = currentFontState.GetWidth( currentFontState.MapCharacter( 'n' ) );
                    if ( em <= 0 )
                        em = 500 * currentFontState.FontSize;
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
                        width = currentFontState.FontSize;
                    if ( c == '\u2004' )
                        width = em / 3;
                    if ( c == '\u2005' )
                        width = em / 4;
                    if ( c == '\u2006' )
                        width = em / 6;
                    if ( c == '\u2007' )
                        width = getCharWidth( ' ' );
                    if ( c == '\u2008' )
                        width = getCharWidth( '.' );
                    if ( c == '\u2009' )
                        width = em / 5;
                    if ( c == '\u200A' )
                        width = 5;
                    if ( c == '\u200B' )
                        width = 100;
                    if ( c == '\u202F' )
                        width = getCharWidth( ' ' ) / 2;
                    if ( c == '\u3000' )
                        width = getCharWidth( ' ' ) * 2;
                }
            }

            return width;
        }

        private bool isSpace( char c )
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

        private bool isNBSP( char c )
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

        private bool isAnySpace( char c )
        {
            bool ret = isSpace( c ) || isNBSP( c );
            return ret;
        }

        private void addSpacedWord( string word, LinkSet ls, int startw,
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
                    && isNBSP( currentWord[ 0 ] ) )
                {
                    // Add an InlineSpace
                    int spaceWidth = getCharWidth( currentWord[ 0 ] );
                    if ( spaceWidth > 0 )
                    {
                        var ispace = new InlineSpace( spaceWidth );
                        extraw += spaceWidth;
                        if ( prevUlState )
                            ispace.setUnderlined( textState.getUnderlined() );
                        if ( prevOlState )
                            ispace.setOverlined( textState.getOverlined() );
                        if ( prevLTState )
                            ispace.setLineThrough( textState.getLineThrough() );

                        if ( addToPending )
                        {
                            pendingAreas.Add( ispace );
                            pendingWidth += spaceWidth;
                        }
                        else
                            addChild( ispace );
                    }
                }
                else
                {
                    var ia = new WordArea( currentFontState, red,
                        green, blue,
                        currentWord,
                        getWordWidth( currentWord ) );
                    ia.setYOffset( placementOffset );
                    ia.setUnderlined( textState.getUnderlined() );
                    prevUlState = textState.getUnderlined();
                    ia.setOverlined( textState.getOverlined() );
                    prevOlState = textState.getOverlined();
                    ia.setLineThrough( textState.getLineThrough() );
                    prevLTState = textState.getLineThrough();
                    ia.setVerticalAlign( vAlign );

                    if ( addToPending )
                    {
                        pendingAreas.Add( ia );
                        pendingWidth += getWordWidth( currentWord );
                    }
                    else
                        addChild( ia );
                    if ( ls != null )
                    {
                        var lr = new Rectangle( startw + extraw, spacew,
                            ia.getContentWidth(),
                            fontState.FontSize );
                        ls.addRect( lr, this, ia );
                    }
                }
            }
        }
    }
}