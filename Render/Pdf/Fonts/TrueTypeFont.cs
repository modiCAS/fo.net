using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts
{
    /// <summary>
    ///     Represents a TrueType font program.
    /// </summary>
    internal class TrueTypeFont : Font, IFontDescriptor
    {
        public const string WinAnsiEncoding = "WinAnsiEncoding";

        /// <summary>
        ///     Wrapper around a Win32 HDC.
        /// </summary>
        private GdiDeviceContent _dc;

        /// <summary>
        ///     List of kerning pairs.
        /// </summary>
        private GdiKerningPairs _kerning;

        private readonly CodePointMapping _mapping =
            CodePointMapping.GetMapping( "WinAnsiEncoding" );

        /// <summary>
        ///     Provides font metrics using the Win32 Api.
        /// </summary>
        private GdiFontMetrics _metrics;

        /// <summary>
        /// </summary>
        protected FontProperties Properties;

        /// <summary>
        ///     Maps a glyph index to a PDF width
        /// </summary>
        private int[] _widths;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="properties"></param>
        public TrueTypeFont( FontProperties properties )
        {
            this.Properties = properties;
            ObtainFontMetrics();
        }

        public PdfArray Array
        {
            get
            {
                var widthsArray = new PdfArray();
                widthsArray.AddArray( Widths );

                return widthsArray;
            }
        }

        /// <summary>
        ///     Creates a <see cref="GdiFontMetrics" /> object from <b>baseFontName</b>
        /// </summary>
        private void ObtainFontMetrics()
        {
            _dc = new GdiDeviceContent();
            GdiFont font = GdiFont.CreateDesignFont(
                Properties.FaceName, Properties.IsBold, Properties.IsItalic, _dc );
            _metrics = font.GetMetrics( _dc );
        }

        private void EnsureWidthsArray()
        {
            if ( _widths == null )
                _widths = _metrics.GetAnsiWidths();
        }

        #region Implementation of Font members

        /// <summary>
        ///     Returns <see cref="PdfFontSubTypeEnum.TrueType" />.
        /// </summary>
        public override PdfFontSubTypeEnum SubType
        {
            get { return PdfFontSubTypeEnum.TrueType; }
        }

        public override string FontName
        {
            get
            {
                // See section 5.5.2 "TrueType fonts" for more details
                if ( Properties.IsBoldItalic )
                    return string.Format( "{0},BoldItalic", Properties.FaceName );
                if ( Properties.IsBold )
                    return string.Format( "{0},Bold", Properties.FaceName );
                if ( Properties.IsItalic )
                    return string.Format( "{0},Italic", Properties.FaceName );
                return Properties.FaceName;
            }
        }

        public override PdfFontTypeEnum Type
        {
            get { return PdfFontTypeEnum.TrueType; }
        }

        public override string Encoding
        {
            get { return WinAnsiEncoding; }
        }

        public override IFontDescriptor Descriptor
        {
            get { return this; }
        }

        public override bool MultiByteFont
        {
            get { return false; }
        }

        public override ushort MapCharacter( char c )
        {
            // TrueType fonts only support the Basic and Extended Latin blocks
            if ( c > byte.MaxValue )
                return (ushort)FirstChar;

            return _mapping.MapCharacter( c );
        }

        public override int Ascender
        {
            get { return _metrics.Ascent; }
        }

        public override int Descender
        {
            get { return _metrics.Descent; }
        }

        public override int CapHeight
        {
            get { return _metrics.CapHeight; }
        }

        public override int FirstChar
        {
            get { return 0; }
        }

        public override int LastChar
        {
            get
            {
                // Only support Latin1 character set
                return 255;
            }
        }

        /// <summary>
        ///     See <see cref="Font.GetWidth(ushort)" />
        /// </summary>
        /// <param name="charIndex">A WinAnsi codepoint.</param>
        /// <returns></returns>
        public override int GetWidth( ushort charIndex )
        {
            EnsureWidthsArray();

            // The widths array is keyed on WinAnsiEncoding codepoint
            return _widths[ charIndex ];
        }

        public override int[] Widths
        {
            get
            {
                EnsureWidthsArray();
                return _widths;
            }
        }

        #endregion

        #region Implementation of IFontDescriptior interface

        public int Flags
        {
            get { return _metrics.Flags; }
        }

        public int[] FontBBox
        {
            get { return _metrics.BoundingBox; }
        }

        public int ItalicAngle
        {
            get { return _metrics.ItalicAngle; }
        }

        public int StemV
        {
            get { return _metrics.StemV; }
        }

        public bool HasKerningInfo
        {
            get
            {
                if ( _kerning == null )
                    _kerning = _metrics.AnsiKerningPairs;
                return _kerning.Count != 0;
            }
        }

        public bool IsEmbeddable
        {
            get { return false; }
        }

        public bool IsSubsettable
        {
            get { return false; }
        }

        public byte[] FontData
        {
            get { return _metrics.GetFontData(); }
        }

        public GdiKerningPairs KerningInfo
        {
            get
            {
                if ( _kerning == null )
                    _kerning = _metrics.AnsiKerningPairs;
                return _kerning;
            }
        }

        #endregion
    }
}