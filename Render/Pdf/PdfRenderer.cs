using System;
using System.Collections;
using System.IO;
using System.Text;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;
using Fonet.Layout.Inline;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;
using Fonet.Render.Pdf.Fonts;

namespace Fonet.Render.Pdf
{
    internal sealed class PdfRenderer
    {
        /// <summary>
        ///     Reusable word area string buffer to reduce memory usage.
        /// </summary>
        /// <remarks>
        ///     TODO: remove use of this.
        /// </remarks>
        private readonly StringBuilder _wordAreaPdf = new StringBuilder();

        /// <summary>
        ///     The current annotation list to add annotations to.
        /// </summary>
        private PdfAnnotList _currentAnnotList;

        /// <summary>
        ///     The horizontal position of the current area container.
        /// </summary>
        private int _currentAreaContainerXPosition;

        /// <summary>
        ///     The current color/gradient to fill shapes with.
        /// </summary>
        private PdfColor _currentFill;

        /// <summary>
        ///     The current (internal) font name.
        /// </summary>
        private string _currentFontName;

        /// <summary>
        ///     The current font size in millipoints.
        /// </summary>
        private int _currentFontSize;

        private float _currentLetterSpacing = float.NaN;

        /// <summary>
        ///     The current page to add annotations to.
        /// </summary>
        private PdfPage _currentPage;

        /// <summary>
        ///     The current stream to add PDF commands to.
        /// </summary>
        private PdfContentStream _currentStream;

        /// <summary>
        ///     The current horizontal position in millipoints from left.
        /// </summary>
        private int _currentXPosition;

        /// <summary>
        ///     The current vertical position in millipoints from bottom.
        /// </summary>
        private int _currentYPosition;

        /// <summary>
        ///     Provides triplet to font resolution.
        /// </summary>
        private FontInfo _fontInfo;

        /// <summary>
        ///     Handles adding base 14 and all system fonts.
        /// </summary>
        private FontSetup _fontSetup;

        /// <summary>
        ///     The IDReferences for this document.
        /// </summary>
        private IDReferences _idReferences;

        /// <summary>
        ///     User specified rendering options.
        /// </summary>
        private PdfRendererOptions _options;

        /// <summary>
        ///     The PDF Document being created.
        /// </summary>
        private PdfCreator _pdfDoc;

        /// <summary>
        ///     The /Resources object of the PDF document being created.
        /// </summary>
        private PdfResources _pdfResources;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private PdfColor _prevLineThroughColor;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevLineThroughSize;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevLineThroughXEndPos;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevLineThroughYEndPos;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private PdfColor _prevOverlineColor;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevOverlineSize;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevOverlineXEndPos;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevOverlineYEndPos;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private PdfColor _prevUnderlineColor;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevUnderlineSize;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevUnderlineXEndPos;

        /// <summary>
        ///     Previous values used for text-decoration drawing.
        /// </summary>
        private int _prevUnderlineYEndPos;

        /// <summary>
        ///     The  width of the previous word.
        /// </summary>
        /// <remarks>
        ///     Used to calculate space between.
        /// </remarks>
        private int _prevWordWidth;

        /// <summary>
        ///     The previous X coordinate of the last word written.
        /// </summary>
        /// <remarks>
        ///     Used to calculate how much space between two words.
        /// </remarks>
        private int _prevWordX;

        /// <summary>
        ///     The previous Y coordinate of the last word written.
        /// </summary>
        /// <remarks>
        ///     Used to decide if we can draw the next word on the same line.
        /// </remarks>
        private int _prevWordY;

        /// <summary>
        ///     True if a TJ command is left to be written.
        /// </summary>
        private bool _textOpen;

        /// <summary>
        ///     Create the PDF renderer.
        /// </summary>
        internal PdfRenderer( Stream stream )
        {
            _pdfDoc = new PdfCreator( stream );
        }

        /// <summary>
        ///     Assigns renderer options to this PdfRenderer
        /// </summary>
        /// <remarks>
        ///     This property will only accept an instance of the PdfRendererOptions class
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     If <i>value</i> is not an instance of PdfRendererOptions
        /// </exception>
        public PdfRendererOptions Options
        {
            set
            {
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                if ( !( value is PdfRendererOptions ) )
                    throw new ArgumentException( "Options must be an instance of PdfRendererOptions" );

                // Guaranteed to work because of above check
                _options = value;
            }
        }

        public void StartRenderer()
        {
            if ( _options != null )
                _pdfDoc.SetOptions( _options );
            _pdfDoc.OutputHeader();
        }

        public void StopRenderer()
        {
            _fontSetup.AddToResources( new PdfFontCreator( _pdfDoc ), _pdfDoc.GetResources() );
            _pdfDoc.OutputTrailer();

            _pdfDoc = null;
            _pdfResources = null;
            _currentStream = null;
            _currentAnnotList = null;
            _currentPage = null;

            _idReferences = null;
            _currentFontName = string.Empty;
            _currentFill = null;
            _prevUnderlineColor = null;
            _prevOverlineColor = null;
            _prevLineThroughColor = null;
            _fontSetup = null;
            _fontInfo = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="fontInfo"></param>
        public void SetupFontInfo( FontInfo fontInfo )
        {
            this._fontInfo = fontInfo;
            _fontSetup = new FontSetup(
                fontInfo, _options == null ? FontType.Link : _options.FontType );
        }

        public void RenderSpanArea( SpanArea area )
        {
            foreach ( Box b in area.GetChildren() )
                b.Render( this ); // column areas
        }

        public void RenderBodyAreaContainer( BodyAreaContainer area )
        {
            int saveY = _currentYPosition;
            int saveX = _currentAreaContainerXPosition;

            if ( area.GetPosition() == Position.Absolute )
            {
                // Y position is computed assuming positive Y axis, adjust for negative postscript one
                _currentYPosition = area.GetYPosition();
                _currentAreaContainerXPosition = area.GetXPosition();
            }
            else if ( area.GetPosition() == Position.Relative )
            {
                _currentYPosition -= area.GetYPosition();
                _currentAreaContainerXPosition += area.GetXPosition();
            }

            _currentXPosition = _currentAreaContainerXPosition;
            int rx = _currentAreaContainerXPosition;
            int ry = _currentYPosition;

            int w = area.GetAllocationWidth();
            int h = area.GetMaxHeight();

            DoBackground( area, rx, ry, w, h );

            // floats & footnotes stuff
            RenderAreaContainer( area.GetBeforeFloatReferenceArea() );
            RenderAreaContainer( area.GetFootnoteReferenceArea() );

            // main reference area
            foreach ( Box b in area.GetMainReferenceArea().GetChildren() )
                b.Render( this ); // span areas

            if ( area.GetPosition() != Position.Static )
            {
                _currentYPosition = saveY;
                _currentAreaContainerXPosition = saveX;
            }
            else
                _currentYPosition -= area.GetHeight();
        }

        public void RenderAreaContainer( AreaContainer area )
        {
            int saveY = _currentYPosition;
            int saveX = _currentAreaContainerXPosition;

            if ( area.GetPosition() == Position.Absolute )
            {
                // XPosition and YPosition give the content rectangle position
                _currentYPosition = area.GetYPosition();
                _currentAreaContainerXPosition = area.GetXPosition();
            }
            else if ( area.GetPosition() == Position.Relative )
            {
                _currentYPosition -= area.GetYPosition();
                _currentAreaContainerXPosition += area.GetXPosition();
            }
            else if ( area.GetPosition() == Position.Static )
            {
                _currentYPosition -= area.GetPaddingTop()
                    + area.GetBorderTopWidth();
            }

            _currentXPosition = _currentAreaContainerXPosition;
            DoFrame( area );

            foreach ( Box b in area.GetChildren() )
                b.Render( this );

            // Restore previous origin
            _currentYPosition = saveY;
            _currentAreaContainerXPosition = saveX;
            if ( area.GetPosition() == Position.Static )
                _currentYPosition -= area.GetHeight();
        }

        public void RenderBlockArea( BlockArea area )
        {
            // KLease: Temporary test to fix block positioning
            // Offset ypos by padding and border widths
            _currentYPosition -= area.GetPaddingTop()
                + area.GetBorderTopWidth();
            DoFrame( area );
            foreach ( Box b in area.GetChildren() )
                b.Render( this );
            _currentYPosition -= area.GetPaddingBottom()
                + area.GetBorderBottomWidth();
        }

        public void RenderLineArea( LineArea area )
        {
            int rx = _currentAreaContainerXPosition + area.GetStartIndent();
            int ry = _currentYPosition;
            int w = area.GetContentWidth();
            int h = area.GetHeight();

            _currentYPosition -= area.GetPlacementOffset();
            _currentXPosition = rx;

            int bl = _currentYPosition;

            foreach ( Box b in area.GetChildren() )
            {
                if ( b is InlineArea )
                {
                    var ia = (InlineArea)b;
                    _currentYPosition = ry - ia.GetYOffset();
                }
                else
                    _currentYPosition = ry - area.GetPlacementOffset();
                b.Render( this );
            }

            _currentYPosition = ry - h;
            _currentXPosition = rx;
        }

        /**
        * add a line to the current stream
        *
        * @param x1 the start x location in millipoints
        * @param y1 the start y location in millipoints
        * @param x2 the end x location in millipoints
        * @param y2 the end y location in millipoints
        * @param th the thickness in millipoints
        * @param r the red component
        * @param g the green component
        * @param b the blue component
        */

        private void AddLine( int x1, int y1, int x2, int y2, int th,
            PdfColor stroke )
        {
            CloseText();

            _currentStream.Write( "ET\nq\n" + stroke.GetColorSpaceOut( false )
                + PdfNumber.DoubleOut( x1 / 1000f ) + " " + PdfNumber.DoubleOut( y1 / 1000f ) + " m "
                + PdfNumber.DoubleOut( x2 / 1000f ) + " " + PdfNumber.DoubleOut( y2 / 1000f ) + " l "
                + PdfNumber.DoubleOut( th / 1000f ) + " w S\n" + "Q\nBT\n" );
        }

        /**
        * add a line to the current stream
        *
        * @param x1 the start x location in millipoints
        * @param y1 the start y location in millipoints
        * @param x2 the end x location in millipoints
        * @param y2 the end y location in millipoints
        * @param th the thickness in millipoints
        * @param rs the rule style
        * @param r the red component
        * @param g the green component
        * @param b the blue component
        */

        private void AddLine( int x1, int y1, int x2, int y2, int th, int rs,
            PdfColor stroke )
        {
            CloseText();
            _currentStream.Write( "ET\nq\n" + stroke.GetColorSpaceOut( false )
                + SetRuleStylePattern( rs ) + PdfNumber.DoubleOut( x1 / 1000f ) + " "
                + PdfNumber.DoubleOut( y1 / 1000f ) + " m " + PdfNumber.DoubleOut( x2 / 1000f ) + " "
                + PdfNumber.DoubleOut( y2 / 1000f ) + " l " + PdfNumber.DoubleOut( th / 1000f ) + " w S\n"
                + "Q\nBT\n" );
        }

        /**
        * add a rectangle to the current stream
        *
        * @param x the x position of left edge in millipoints
        * @param y the y position of top edge in millipoints
        * @param w the width in millipoints
        * @param h the height in millipoints
        * @param stroke the stroke color/gradient
        */

        private void AddRect( int x, int y, int w, int h, PdfColor stroke )
        {
            CloseText();
            _currentStream.Write( "ET\nq\n" + stroke.GetColorSpaceOut( false )
                + PdfNumber.DoubleOut( x / 1000f ) + " " + PdfNumber.DoubleOut( y / 1000f ) + " "
                + PdfNumber.DoubleOut( w / 1000f ) + " " + PdfNumber.DoubleOut( h / 1000f ) + " re s\n"
                + "Q\nBT\n" );
        }

        /**
        * add a filled rectangle to the current stream
        *
        * @param x the x position of left edge in millipoints
        * @param y the y position of top edge in millipoints
        * @param w the width in millipoints
        * @param h the height in millipoints
        * @param fill the fill color/gradient
        * @param stroke the stroke color/gradient
        */

        private void AddRect( int x, int y, int w, int h, PdfColor stroke,
            PdfColor fill )
        {
            CloseText();
            _currentStream.Write( "ET\nq\n" + fill.GetColorSpaceOut( true )
                + stroke.GetColorSpaceOut( false ) + PdfNumber.DoubleOut( x / 1000f )
                + " " + PdfNumber.DoubleOut( y / 1000f ) + " " + PdfNumber.DoubleOut( w / 1000f ) + " "
                + PdfNumber.DoubleOut( h / 1000f ) + " re b\n" + "Q\nBT\n" );
        }

        /**
        * add a filled rectangle to the current stream
        *
        * @param x the x position of left edge in millipoints
        * @param y the y position of top edge in millipoints
        * @param w the width in millipoints
        * @param h the height in millipoints
        * @param fill the fill color/gradient
        */

        private void AddFilledRect( int x, int y, int w, int h,
            PdfColor fill )
        {
            CloseText();
            _currentStream.Write( "ET\nq\n" + fill.GetColorSpaceOut( true )
                + PdfNumber.DoubleOut( x / 1000f ) + " " + PdfNumber.DoubleOut( y / 1000f ) + " "
                + PdfNumber.DoubleOut( w / 1000f ) + " " + PdfNumber.DoubleOut( h / 1000f ) + " re f\n"
                + "Q\nBT\n" );
        }

        /**
        * render image area to PDF
        *
        * @param area the image area to render
        */

        public void RenderImageArea( ImageArea area )
        {
            int x = _currentXPosition + area.GetXOffset();
            int y = _currentYPosition;
            int w = area.GetContentWidth();
            int h = area.GetHeight();

            _currentYPosition -= h;

            FonetImage img = area.GetImage();

            PdfXObject xobj = _pdfDoc.AddImage( img );
            CloseText();

            _currentStream.Write( "ET\nq\n" + PdfNumber.DoubleOut( w / 1000f ) + " 0 0 "
                + PdfNumber.DoubleOut( h / 1000f ) + " "
                + PdfNumber.DoubleOut( x / 1000f ) + " "
                + PdfNumber.DoubleOut( ( y - h ) / 1000f ) + " cm\n" + "/" + xobj.Name.Name
                + " Do\nQ\nBT\n" );

            _currentXPosition += area.GetContentWidth();
        }

        /**
        * render a foreign object area
        */

        public void RenderForeignObjectArea( ForeignObjectArea area )
        {
            // if necessary need to scale and align the content
            _currentXPosition = _currentXPosition + area.GetXOffset();
            // TODO: why was this here? this.currentYPosition = this.currentYPosition;
            switch ( area.GetAlign() )
            {
            case TextAlign.Start:
                break;
            case TextAlign.End:
                break;
            case TextAlign.Center:
            case TextAlign.Justify:
                break;
            }
            switch ( area.GetVerticalAlign() )
            {
            case VerticalAlign.Baseline:
                break;
            case VerticalAlign.Middle:
                break;
            case VerticalAlign.Sub:
                break;
            case VerticalAlign.Super:
                break;
            case VerticalAlign.TextTop:
                break;
            case VerticalAlign.TextBottom:
                break;
            case VerticalAlign.Top:
                break;
            case VerticalAlign.Bottom:
                break;
            }
            CloseText();

            // in general the content will not be text
            _currentStream.Write( "ET\n" );
            // align and scale
            _currentStream.Write( "q\n" );
            switch ( area.ScalingMethod() )
            {
            case Scaling.Uniform:
                break;
            case Scaling.NonUniform:
                break;
            }
            // if the overflow is auto (default), scroll or visible
            // then the contents should not be clipped, since this
            // is considered a printing medium.
            switch ( area.GetOverflow() )
            {
            case Overflow.Visible:
            case Overflow.Scroll:
            case Overflow.Auto:
                break;
            case Overflow.Hidden:
                break;
            }

            area.GetObject().Render( this );
            _currentStream.Write( "Q\n" );
            _currentStream.Write( "BT\n" );
            _currentXPosition += area.GetEffectiveWidth();
            // this.currentYPosition -= area.getEffectiveHeight();
        }

        /**
        * render inline area to PDF
        *
        * @param area inline area to render
        */

        public void RenderWordArea( WordArea area )
        {
            // TODO: I don't understand why we are locking the private member
            // _wordAreaPDF.  Maybe this string buffer was originally static? (MG)
            lock ( _wordAreaPdf )
            {
                StringBuilder pdf = _wordAreaPdf;
                pdf.Length = 0;

                GdiKerningPairs kerning = null;
                var kerningAvailable = false;

                // If no options are supplied, by default we do not enable kerning
                if ( _options != null && _options.Kerning )
                {
                    kerning = area.GetFontState().Kerning;
                    if ( kerning != null && kerning.Count > 0 )
                        kerningAvailable = true;
                }

                string name = area.GetFontState().FontName;
                int size = area.GetFontState().FontSize;

                // This assumes that *all* CIDFonts use a /ToUnicode mapping
                var font = (Font)area.GetFontState().FontInfo.GetFontByName( name );
                bool useMultiByte = font.MultiByteFont;

                string startText = useMultiByte ? "<" : "(";
                string endText = useMultiByte ? "> " : ") ";

                if ( !name.Equals( _currentFontName ) || size != _currentFontSize )
                {
                    CloseText();

                    _currentFontName = name;
                    _currentFontSize = size;
                    pdf = pdf.Append( "/" + name + " " +
                        PdfNumber.DoubleOut( size / 1000f ) + " Tf\n" );
                }

                // Do letter spacing (must be outside of [...] TJ]
                float letterspacing = area.GetFontState().LetterSpacing / 1000f;
                if ( letterspacing != _currentLetterSpacing )
                {
                    _currentLetterSpacing = letterspacing;
                    CloseText();
                    pdf.Append( PdfNumber.DoubleOut( letterspacing ) );
                    pdf.Append( " Tc\n" );
                }

                PdfColor areaColor = _currentFill;

                if ( areaColor == null || areaColor.GetRed() != area.GetRed()
                    || areaColor.GetGreen() != area.GetGreen()
                    || areaColor.GetBlue() != area.GetBlue() )
                {
                    areaColor = new PdfColor( area.GetRed(),
                        area.GetGreen(),
                        area.GetBlue() );


                    CloseText();
                    _currentFill = areaColor;
                    pdf.Append( _currentFill.GetColorSpaceOut( true ) );
                }


                int rx = _currentXPosition;
                int bl = _currentYPosition;

                AddWordLines( area, rx, bl, size, areaColor );

                if ( !_textOpen || bl != _prevWordY )
                {
                    CloseText();

                    pdf.Append( "1 0 0 1 " + PdfNumber.DoubleOut( rx / 1000f ) +
                        " " + PdfNumber.DoubleOut( bl / 1000f ) + " Tm [" + startText );
                    _prevWordY = bl;
                    _textOpen = true;
                }
                else
                {
                    // express the space between words in thousandths of an em
                    int space = _prevWordX - rx + _prevWordWidth;
                    float emDiff = space / (float)_currentFontSize * 1000f;
                    // this prevents a problem in Acrobat Reader where large
                    // numbers cause text to disappear or default to a limit
                    if ( emDiff < -33000 )
                    {
                        CloseText();

                        pdf.Append( "1 0 0 1 " + PdfNumber.DoubleOut( rx / 1000f ) +
                            " " + PdfNumber.DoubleOut( bl / 1000f ) + " Tm [" + startText );
                        _textOpen = true;
                    }
                    else
                    {
                        pdf.Append( PdfNumber.DoubleOut( emDiff ) );
                        pdf.Append( " " );
                        pdf.Append( startText );
                    }
                }
                _prevWordWidth = area.GetContentWidth();
                _prevWordX = rx;

                string s;
                if ( area.GetPageNumberID() != null )
                {
                    // This text is a page number, so resolve it
                    s = _idReferences.GetPageNumber( area.GetPageNumberID() );
                    if ( s == null )
                        s = string.Empty;
                }
                else
                    s = area.GetText();

                int wordLength = s.Length;
                for ( var index = 0; index < wordLength; index++ )
                {
                    ushort ch = area.GetFontState().MapCharacter( s[ index ] );

                    if ( !useMultiByte )
                    {
                        if ( ch > 127 )
                        {
                            pdf.Append( "\\" );
                            pdf.Append( Convert.ToString( ch, 8 ) );
                        }
                        else
                        {
                            switch ( ch )
                            {
                            case '(':
                            case ')':
                            case '\\':
                                pdf.Append( "\\" );
                                break;
                            }
                            pdf.Append( (char)ch );
                        }
                    }
                    else
                        pdf.Append( GetUnicodeString( ch ) );

                    if ( kerningAvailable && index + 1 < wordLength )
                    {
                        ushort ch2 = area.GetFontState().MapCharacter( s[ index + 1 ] );
                        AddKerning( pdf, ch, ch2, kerning, startText, endText );
                    }
                }
                pdf.Append( endText );

                _currentStream.Write( pdf.ToString() );

                _currentXPosition += area.GetContentWidth();
            }
        }

        /**
        * Convert a char to a multibyte hex representation
        */

        private string GetUnicodeString( ushort c )
        {
            var sb = new StringBuilder( 4 );

            byte[] uniBytes = Encoding.BigEndianUnicode.GetBytes( new[] { (char)c } );

            foreach ( byte b in uniBytes )
            {
                string hexString = Convert.ToString( b, 16 );
                if ( hexString.Length == 1 )
                    sb.Append( "0" );
                sb.Append( hexString );
            }

            return sb.ToString();
        }

        /**
        * Checks to see if we have some text rendering commands open
        * still and writes out the TJ command to the stream if we do
        */

        private void CloseText()
        {
            if ( _textOpen )
            {
                _currentStream.Write( "] TJ\n" );
                _textOpen = false;
                _prevWordX = 0;
                _prevWordY = 0;
            }
        }

        private void AddKerning( StringBuilder buf, ushort leftIndex, ushort rightIndex,
            GdiKerningPairs kerning, string startText, string endText )
        {
            if ( kerning.HasPair( leftIndex, rightIndex ) )
            {
                int width = kerning[ leftIndex, rightIndex ];
                buf.Append( endText ).Append( -width ).Append( ' ' ).Append( startText );
            }
        }


        public void Render( Page page )
        {
            _idReferences = page.GetIDReferences();
            _pdfResources = _pdfDoc.GetResources();
            _pdfDoc.SetIDReferences( _idReferences );
            RenderPage( page );
            _pdfDoc.Output();
        }


        /**
        * render page into PDF
        *
        * @param page page to render
        */
        public void RenderPage( Page page )
        {
            _currentStream = _pdfDoc.MakeContentStream();
            BodyAreaContainer body = page.GetBody();
            AreaContainer before = page.GetBefore();
            AreaContainer after = page.GetAfter();
            AreaContainer start = page.GetStart();
            AreaContainer end = page.GetEnd();

            _currentFontName = "";
            _currentFontSize = 0;
            _currentLetterSpacing = float.NaN;

            _currentStream.Write( "BT\n" );

            RenderBodyAreaContainer( body );

            if ( before != null )
                RenderAreaContainer( before );

            if ( after != null )
                RenderAreaContainer( after );

            if ( start != null )
                RenderAreaContainer( start );

            if ( end != null )
                RenderAreaContainer( end );
            CloseText();

            // Bug fix for issue 1823
            _currentLetterSpacing = float.NaN;

            float w = page.GetWidth();
            float h = page.GetHeight();
            _currentStream.Write( "ET\n" );

            _currentPage = _pdfDoc.MakePage(
                _pdfResources, _currentStream,
                Convert.ToInt32( Math.Round( w / 1000 ) ),
                Convert.ToInt32( Math.Round( h / 1000 ) ), page );

            if ( page.HasLinks() || _currentAnnotList != null )
            {
                if ( _currentAnnotList == null )
                    _currentAnnotList = _pdfDoc.MakeAnnotList();
                _currentPage.SetAnnotList( _currentAnnotList );

                ArrayList lsets = page.GetLinkSets();
                foreach ( LinkSet linkSet in lsets )
                {
                    linkSet.Align();
                    string dest = linkSet.GetDest();
                    int linkType = linkSet.GetLinkType();
                    ArrayList rsets = linkSet.GetRects();
                    foreach ( LinkedRectangle lrect in rsets )
                    {
                        _currentAnnotList.Add( _pdfDoc.MakeLink( lrect.GetRectangle(),
                            dest, linkType ).GetReference() );
                    }
                }
                _currentAnnotList = null;
            }
            else
            {
                // just to be on the safe side
                _currentAnnotList = null;
            }

            // ensures that color is properly reset for blocks that carry over pages
            _currentFill = null;
        }

        /**
        * defines a string containing dashArray and dashPhase for the rule style
        */

        private string SetRuleStylePattern( int style )
        {
            var rs = "";
            switch ( style )
            {
            case RuleStyle.Solid:
                rs = "[] 0 d ";
                break;
            case RuleStyle.Dashed:
                rs = "[3 3] 0 d ";
                break;
            case RuleStyle.Dotted:
                rs = "[1 3] 0 d ";
                break;
            case RuleStyle.Double:
                rs = "[] 0 d ";
                break;
            default:
                rs = "[] 0 d ";
                break;
            }
            return rs;
        }

        private void DoFrame( Area area )
        {
            int w, h;
            int rx = _currentAreaContainerXPosition;
            w = area.GetContentWidth();
            if ( area is BlockArea )
                rx += ( (BlockArea)area ).GetStartIndent();
            h = area.GetContentHeight();
            int ry = _currentYPosition;

            rx = rx - area.GetPaddingLeft();
            ry = ry + area.GetPaddingTop();
            w = w + area.GetPaddingLeft() + area.GetPaddingRight();
            h = h + area.GetPaddingTop() + area.GetPaddingBottom();

            DoBackground( area, rx, ry, w, h );

            BorderAndPadding bp = area.GetBorderAndPadding();

            int left = area.GetBorderLeftWidth();
            int right = area.GetBorderRightWidth();
            int top = area.GetBorderTopWidth();
            int bottom = area.GetBorderBottomWidth();

            // If style is solid, use filled rectangles
            if ( top != 0 )
            {
                AddFilledRect( rx, ry, w, top,
                    new PdfColor( bp.GetBorderColor( BorderAndPadding.Top ) ) );
            }
            if ( left != 0 )
            {
                AddFilledRect( rx - left, ry - h - bottom, left, h + top + bottom,
                    new PdfColor( bp.GetBorderColor( BorderAndPadding.Left ) ) );
            }
            if ( right != 0 )
            {
                AddFilledRect( rx + w, ry - h - bottom, right, h + top + bottom,
                    new PdfColor( bp.GetBorderColor( BorderAndPadding.Right ) ) );
            }
            if ( bottom != 0 )
            {
                AddFilledRect( rx, ry - h - bottom, w, bottom,
                    new PdfColor( bp.GetBorderColor( BorderAndPadding.Bottom ) ) );
            }
        }

        /// <summary>
        ///     Renders an area's background.
        /// </summary>
        /// <param name="area">The area whose background is to be rendered.</param>
        /// <param name="x">The x position of the left edge in millipoints.</param>
        /// <param name="y">The y position of top edge in millipoints.</param>
        /// <param name="w">The width in millipoints.</param>
        /// <param name="h">The height in millipoints.</param>
        private void DoBackground( Area area, int x, int y, int w, int h )
        {
            if ( h == 0 || w == 0 )
                return;

            BackgroundProps props = area.GetBackground();
            if ( props == null )
                return;

            if ( props.BackColor.Alpha == 0 )
                AddFilledRect( x, y, w, -h, new PdfColor( props.BackColor ) );

            if ( props.BackImage != null )
            {
                int imgW = props.BackImage.Width * 1000;
                int imgH = props.BackImage.Height * 1000;

                int dx = x;
                int dy = y;
                int endX = x + w;
                int endY = y - h;
                int clipW = w % imgW;
                int clipH = h % imgH;

                var repeatX = true;
                var repeatY = true;
                switch ( props.BackRepeat )
                {
                case BackgroundRepeat.Repeat:
                    break;
                case BackgroundRepeat.RepeatX:
                    repeatY = false;
                    break;
                case BackgroundRepeat.RepeatY:
                    repeatX = false;
                    break;
                case BackgroundRepeat.NoRepeat:
                    repeatX = false;
                    repeatY = false;
                    break;
                case BackgroundRepeat.Inherit:
                    break;
                default:
                    FonetDriver.ActiveDriver.FireFonetWarning( "Ignoring invalid background-repeat property" );
                    break;
                }

                while ( dy > endY )
                {
                    // looping through rows 
                    while ( dx < endX )
                    {
                        // looping through cols 
                        if ( dx + imgW <= endX )
                        {
                            // no x clipping 
                            if ( dy - imgH >= endY )
                            {
                                // no x clipping, no y clipping 
                                DrawImageScaled( dx, dy, imgW, imgH, props.BackImage );
                            }
                            else
                            {
                                // no x clipping, y clipping 
                                DrawImageClipped( dx, dy, 0, 0, imgW, clipH, props.BackImage );
                            }
                        }
                        else
                        {
                            // x clipping
                            if ( dy - imgH >= endY )
                            {
                                // x clipping, no y clipping 
                                DrawImageClipped( dx, dy, 0, 0, clipW, imgH, props.BackImage );
                            }
                            else
                            {
                                // x clipping, y clipping
                                DrawImageClipped( dx, dy, 0, 0, clipW, clipH, props.BackImage );
                            }
                        }

                        if ( repeatX )
                            dx += imgW;
                        else
                            break;
                    } // end looping through cols

                    dx = x;

                    if ( repeatY )
                        dy -= imgH;
                    else
                        break;
                } // end looping through rows 
            }
        }

        /// <summary>
        ///     Renders an image, rendered at the image's intrinsic size.
        ///     This by default calls drawImageScaled() with the image's
        ///     intrinsic width and height, but implementations may
        ///     override this method if it can provide a more efficient solution.
        /// </summary>
        /// <param name="x">The x position of left edge in millipoints.</param>
        /// <param name="y">The y position of top edge in millipoints.</param>
        /// <param name="image">The image to be rendered.</param>
        private void DrawImage( int x, int y, FonetImage image )
        {
            int w = image.Width * 1000;
            int h = image.Height * 1000;
            DrawImageScaled( x, y, w, h, image );
        }

        /// <summary>
        ///     Renders an image, scaling it to the given width and height.
        ///     If the scaled width and height is the same intrinsic size
        ///     of the image, the image is not scaled
        /// </summary>
        /// <param name="x">The x position of left edge in millipoints.</param>
        /// <param name="y">The y position of top edge in millipoints.</param>
        /// <param name="w">The width in millipoints.</param>
        /// <param name="h">The height in millipoints.</param>
        /// <param name="image">The image to be rendered.</param>
        private void DrawImageScaled(
            int x, int y, int w, int h, FonetImage image )
        {
            PdfXObject xobj = _pdfDoc.AddImage( image );
            CloseText();

            _currentStream.Write( "ET\nq\n" + PdfNumber.DoubleOut( w / 1000f ) + " 0 0 "
                + PdfNumber.DoubleOut( h / 1000f ) + " "
                + PdfNumber.DoubleOut( x / 1000f ) + " "
                + PdfNumber.DoubleOut( ( y - h ) / 1000f ) + " cm\n" + "/" + xobj.Name.Name
                + " Do\nQ\nBT\n" );
        }

        /// <summary>
        ///     Renders an image, clipping it as specified.
        /// </summary>
        /// <param name="x">The x position of left edge in millipoints.</param>
        /// <param name="y">The y position of top edge in millipoints.</param>
        /// <param name="clipX">The left edge of the clip in millipoints.</param>
        /// <param name="clipY">The top edge of the clip in millipoints.</param>
        /// <param name="clipW">The clip width in millipoints.</param>
        /// <param name="clipH">The clip height in millipoints.</param>
        /// <param name="image">The image to be rendered.</param>
        private void DrawImageClipped(
            int x, int y, int clipX, int clipY,
            int clipW, int clipH, FonetImage image )
        {
            float cx1 = x / 1000f;
            float cy1 = ( (float)y - clipH ) / 1000f;

            float cx2 = ( (float)x + clipW ) / 1000f;
            float cy2 = y / 1000f;

            int imgX = x - clipX;
            int imgY = y - clipY;

            int imgW = image.Width * 1000;
            int imgH = image.Height * 1000;

            PdfXObject xobj = _pdfDoc.AddImage( image );
            CloseText();

            _currentStream.Write( "ET\nq\n" +
                // clipping
                PdfNumber.DoubleOut( cx1 ) + " " + PdfNumber.DoubleOut( cy1 ) + " m\n" +
                PdfNumber.DoubleOut( cx2 ) + " " + PdfNumber.DoubleOut( cy1 ) + " l\n" +
                PdfNumber.DoubleOut( cx2 ) + " " + PdfNumber.DoubleOut( cy2 ) + " l\n" +
                PdfNumber.DoubleOut( cx1 ) + " " + PdfNumber.DoubleOut( cy2 ) + " l\n" +
                "W\n" +
                "n\n" +
                // image matrix
                PdfNumber.DoubleOut( imgW / 1000f ) + " 0 0 " +
                PdfNumber.DoubleOut( imgH / 1000f ) + " " +
                PdfNumber.DoubleOut( imgX / 1000f ) + " " +
                PdfNumber.DoubleOut( ( (float)imgY - imgH ) / 1000f ) + " cm\n" +
                "s\n" +
                // the image itself
                "/" + xobj.Name.Name + " Do\nQ\nBT\n" );
        }


        /**
         * render display space
         *
         * @param space the display space to render
         */

        public void RenderDisplaySpace( DisplaySpace space )
        {
            int d = space.GetSize();
            _currentYPosition -= d;
        }

        private void AddWordLines( WordArea area, int rx, int bl, int size,
            PdfColor theAreaColor )
        {
            if ( area.GetUnderlined() )
            {
                int yPos = bl - size / 10;
                AddLine( rx, yPos, rx + area.GetContentWidth(), yPos, size / 14,
                    theAreaColor );
                // save position for underlining a following InlineSpace
                _prevUnderlineXEndPos = rx + area.GetContentWidth();
                _prevUnderlineYEndPos = yPos;
                _prevUnderlineSize = size / 14;
                _prevUnderlineColor = theAreaColor;
            }

            if ( area.GetOverlined() )
            {
                int yPos = bl + area.GetFontState().Ascender + size / 10;
                AddLine( rx, yPos, rx + area.GetContentWidth(), yPos, size / 14,
                    theAreaColor );
                _prevOverlineXEndPos = rx + area.GetContentWidth();
                _prevOverlineYEndPos = yPos;
                _prevOverlineSize = size / 14;
                _prevOverlineColor = theAreaColor;
            }

            if ( area.GetLineThrough() )
            {
                int yPos = bl + area.GetFontState().Ascender * 3 / 8;
                AddLine( rx, yPos, rx + area.GetContentWidth(), yPos, size / 14,
                    theAreaColor );
                _prevLineThroughXEndPos = rx + area.GetContentWidth();
                _prevLineThroughYEndPos = yPos;
                _prevLineThroughSize = size / 14;
                _prevLineThroughColor = theAreaColor;
            }
        }

        /**
         * render inline space
         *
         * @param space space to render
         */

        public void RenderInlineSpace( InlineSpace space )
        {
            _currentXPosition += space.GetSize();
            if ( space.GetUnderlined() )
            {
                if ( _prevUnderlineColor != null )
                {
                    AddLine( _prevUnderlineXEndPos, _prevUnderlineYEndPos,
                        _prevUnderlineXEndPos + space.GetSize(),
                        _prevUnderlineYEndPos, _prevUnderlineSize,
                        _prevUnderlineColor );
                    // save position for a following InlineSpace
                    _prevUnderlineXEndPos = _prevUnderlineXEndPos + space.GetSize();
                }
            }
            if ( space.GetOverlined() )
            {
                if ( _prevOverlineColor != null )
                {
                    AddLine( _prevOverlineXEndPos, _prevOverlineYEndPos,
                        _prevOverlineXEndPos + space.GetSize(),
                        _prevOverlineYEndPos, _prevOverlineSize,
                        _prevOverlineColor );
                    _prevOverlineXEndPos = _prevOverlineXEndPos + space.GetSize();
                }
            }
            if ( space.GetLineThrough() )
            {
                if ( _prevLineThroughColor != null )
                {
                    AddLine( _prevLineThroughXEndPos, _prevLineThroughYEndPos,
                        _prevLineThroughXEndPos + space.GetSize(),
                        _prevLineThroughYEndPos, _prevLineThroughSize,
                        _prevLineThroughColor );
                    _prevLineThroughXEndPos = _prevLineThroughXEndPos + space.GetSize();
                }
            }
        }

        /**
         * render leader area
         *
         * @param area area to render
         */

        public void RenderLeaderArea( LeaderArea area )
        {
            int rx = _currentXPosition;
            int ry = _currentYPosition;
            int w = area.GetContentWidth();
            int h = area.GetHeight();
            int th = area.GetRuleThickness();
            int st = area.GetRuleStyle();

            // checks whether thickness is = 0, because of bug in pdf (or where?),
            // a line with thickness 0 is still displayed
            if ( th != 0 )
            {
                switch ( st )
                {
                case RuleStyle.Double:
                    AddLine( rx, ry, rx + w, ry, th / 3, st,
                        new PdfColor( area.GetRed(), area.GetGreen(),
                            area.GetBlue() ) );
                    AddLine( rx, ry + 2 * th / 3, rx + w, ry + 2 * th / 3,
                        th / 3, st,
                        new PdfColor( area.GetRed(), area.GetGreen(),
                            area.GetBlue() ) );
                    break;
                case RuleStyle.Groove:
                    AddLine( rx, ry, rx + w, ry, th / 2, st,
                        new PdfColor( area.GetRed(), area.GetGreen(),
                            area.GetBlue() ) );
                    AddLine( rx, ry + th / 2, rx + w, ry + th / 2, th / 2, st,
                        new PdfColor( 255, 255, 255 ) );
                    break;
                case RuleStyle.Ridge:
                    AddLine( rx, ry, rx + w, ry, th / 2, st,
                        new PdfColor( 255, 255, 255 ) );
                    AddLine( rx, ry + th / 2, rx + w, ry + th / 2, th / 2, st,
                        new PdfColor( area.GetRed(), area.GetGreen(),
                            area.GetBlue() ) );
                    break;
                default:
                    AddLine( rx, ry, rx + w, ry, th, st,
                        new PdfColor( area.GetRed(), area.GetGreen(),
                            area.GetBlue() ) );
                    break;
                }
                _currentXPosition += area.GetContentWidth();
                _currentYPosition += th;
            }
        }
    }
}