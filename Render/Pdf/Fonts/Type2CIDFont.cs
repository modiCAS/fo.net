using System;
using System.Collections;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts
{
    /// <summary>
    ///     A Type 2 CIDFont is a font whose glyph descriptions are based on the
    ///     TrueType font format.
    /// </summary>
    /// <remarks>
    ///     TODO: Support font subsetting
    /// </remarks>
    internal class Type2CidFont : CidFont, IFontDescriptor
    {
        public const string IdentityHEncoding = "Identity-H";

        /// <summary>
        ///     Windows font name, e.g. 'Arial Bold'
        /// </summary>
        protected string BaseFontName;

        /// <summary>
        ///     Wrapper around a Win32 HDC.
        /// </summary>
        protected GdiDeviceContent Dc;

        /// <summary>
        ///     List of kerning pairs.
        /// </summary>
        protected GdiKerningPairs Kerning;

        /// <summary>
        ///     Provides font metrics using the Win32 Api.
        /// </summary>
        protected GdiFontMetrics Metrics;

        /// <summary>
        /// </summary>
        protected FontProperties Properties;

        /// <summary>
        ///     Maps character code to glyph index.  The array is based on the
        ///     value of <see cref="FirstChar" />.
        /// </summary>
        protected GdiUnicodeRanges UnicodeRanges;

        /// <summary>
        ///     Maps a glyph index to a character code.
        /// </summary>
        protected SortedList UsedGlyphs;

        /// <summary>
        ///     Maps a glyph index to a PDF width
        /// </summary>
        protected int[] WidthArray;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        public Type2CidFont( FontProperties properties )
        {
            this.Properties = properties;
            BaseFontName = properties.FaceName.Replace( " ", "-" );
            UsedGlyphs = new SortedList();

            ObtainFontMetrics();
        }

        /// <summary>
        ///     Creates a <see cref="GdiFontMetrics" /> object from <b>baseFontName</b>
        /// </summary>
        private void ObtainFontMetrics()
        {
            Dc = new GdiDeviceContent();
            GdiFont font = GdiFont.CreateDesignFont(
                Properties.FaceName, Properties.IsBold, Properties.IsItalic, Dc );
            UnicodeRanges = new GdiUnicodeRanges( Dc );
            Metrics = font.GetMetrics( Dc );
        }

        /// <summary>
        ///     Class destructor.
        /// </summary>
        ~Type2CidFont()
        {
            Dc.Dispose();
        }

        #region Implementation of CIDFont members

        public override string CidBaseFont
        {
            get { return BaseFontName; }
        }

        public override PdfWArray WArray
        {
            get
            {
                // The widths array for a font using the Unicode encoding is enormous.
                // Instead of encoding the entire widths array, we generated a subset 
                // based on the used glyphs only.
                IList indicies = UsedGlyphs.GetKeyList();
                int[] subsetWidths = GetSubsetWidthsArray( indicies );

                var widthsArray = new PdfWArray( (int)indicies[ 0 ] );
                widthsArray.AddEntry( subsetWidths );

                return widthsArray;
            }
        }

        public override IDictionary CMapEntries
        {
            get
            {
                // The usedGlyphs sorted list maps glyph indices to unicode values
                return (IDictionary)UsedGlyphs.Clone();
            }
        }

        private int[] GetSubsetWidthsArray( IList indicies )
        {
            var firstIndex = (int)indicies[ 0 ];
            var lastIndex = (int)indicies[ indicies.Count - 1 ];

            // Allocate space for glyph subset
            var subsetWidths = new int[ lastIndex - firstIndex + 1 ];
            Array.Clear( subsetWidths, 0, subsetWidths.Length );

            var firstChar = (char)Metrics.FirstChar;
            foreach ( DictionaryEntry entry in UsedGlyphs )
            {
                var c = (char)entry.Value;
                var glyphIndex = (int)entry.Key;
                subsetWidths[ glyphIndex - firstIndex ] = WidthArray[ glyphIndex ];
            }
            return subsetWidths;
        }

        #endregion

        #region Implementation of Font members

        /// <summary>
        ///     Returns <see cref="PdfFontSubTypeEnum.CidFontType2" />.
        /// </summary>
        public override PdfFontSubTypeEnum SubType
        {
            get { return PdfFontSubTypeEnum.CidFontType2; }
        }

        public override string FontName
        {
            get { return BaseFontName; }
        }

        public override string Encoding
        {
            get { return IdentityHEncoding; }
        }

        public override IFontDescriptor Descriptor
        {
            get { return this; }
        }

        public override bool MultiByteFont
        {
            get { return true; }
        }

        public override ushort MapCharacter( char c )
        {
            // Obtain glyph index from Unicode character
            ushort glyphIndex = UnicodeRanges.MapCharacter( c );

            AddGlyphToCharMapping( glyphIndex, c );

            return glyphIndex;
        }

        protected virtual void AddGlyphToCharMapping( ushort glyphIndex, char c )
        {
            // The usedGlyphs dictionary permits a reverse lookup (glyph index to char)
            if ( !UsedGlyphs.ContainsKey( (int)glyphIndex ) )
                UsedGlyphs.Add( (int)glyphIndex, c );
        }

        public override int Ascender
        {
            get { return Metrics.Ascent; }
        }

        public override int Descender
        {
            get { return Metrics.Descent; }
        }

        public override int CapHeight
        {
            get { return Metrics.CapHeight; }
        }

        public override int FirstChar
        {
            get { return Metrics.FirstChar; }
        }

        public override int LastChar
        {
            get { return Metrics.LastChar; }
        }

        public override int GetWidth( ushort charIndex )
        {
            EnsureWidthsArray();

            // The widths array is keyed on character code, not glyph index
            return WidthArray[ charIndex ];
        }

        public override int[] Widths
        {
            get
            {
                EnsureWidthsArray();
                return WidthArray;
            }
        }

        protected void EnsureWidthsArray()
        {
            if ( WidthArray == null )
                WidthArray = Metrics.GetWidths();
        }

        #endregion

        #region Implementation of IFontDescriptior interface

        public int Flags
        {
            get { return Metrics.Flags; }
        }

        public int[] FontBBox
        {
            get { return Metrics.BoundingBox; }
        }

        public int ItalicAngle
        {
            get { return Metrics.ItalicAngle; }
        }

        public int StemV
        {
            get { return Metrics.StemV; }
        }

        public bool HasKerningInfo
        {
            get
            {
                if ( Kerning == null )
                    Kerning = Metrics.KerningPairs;
                return Kerning.Count != 0;
            }
        }

        public bool IsEmbeddable
        {
            get { return Metrics.IsEmbeddable; }
        }

        public bool IsSubsettable
        {
            get { return Metrics.IsSubsettable; }
        }

        public virtual byte[] FontData
        {
            get { return Metrics.GetFontData(); }
        }

        public GdiKerningPairs KerningInfo
        {
            get
            {
                if ( Kerning == null )
                    Kerning = Metrics.KerningPairs;
                return Kerning;
            }
        }

        #endregion
    }
}