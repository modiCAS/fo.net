using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Fonet.Pdf.Gdi.Font;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     Class that obtains OutlineTextMetrics for a TrueType font
    /// </summary>
    /// <example>
    /// </example>
    public class GdiFontMetrics
    {
        public const long GdiError = 0xFFFFFFFFL;
        private readonly PdfUnitConverter _converter;
        private GdiFont _currentFont;

        private byte[] _data;
        private readonly GdiDeviceContent _dc;

        private HeaderTable _head;
        private HorizontalHeaderTable _hhea;
        private HorizontalMetricsTable _hmtx;
        private KerningTable _kern;
        private Os2Table _os2;
        private PostTable _post;
        private readonly GdiUnicodeRanges _ranges;

        private readonly FontFileReader _reader;

        internal GdiFontMetrics( GdiDeviceContent dc, GdiFont currentFont )
        {
            if ( dc.Handle == IntPtr.Zero )
                throw new ArgumentNullException( "dc", "Handle to device context cannot be null" );
            if ( dc.GetCurrentObject( GdiDcObject.Font ) == IntPtr.Zero )
                throw new ArgumentException( "dc", "No font selected into supplied device context" );
            this._dc = dc;
            this._currentFont = currentFont;

            // FontFileReader requires the font facename because the font may exist in 
            // a TrueType collection.
            var builder = new StringBuilder( 255 );
            LibWrapper.GetTextFace( dc.Handle, builder.Capacity, builder );
            FaceName = builder.ToString();

            _ranges = new GdiUnicodeRanges( dc );
            _reader = new FontFileReader( new MemoryStream( GetFontData() ), FaceName );
            _converter = new PdfUnitConverter( EmSquare );

            // After we have cached the font data, we can safely delete the resource
            currentFont.Dispose();
        }

        /// <summary>
        ///     Retrieves the typeface name of the font that is selected into the
        ///     device context supplied to the GdiFontMetrics constructor.
        /// </summary>
        public string FaceName { get; private set; }

        /// <summary>
        ///     Specifies the number of logical units defining the x- or y-dimension
        ///     of the em square for this font.  The common value for EmSquare is 2048.
        /// </summary>
        /// <remarks>
        ///     The number of units in the x- and y-directions are always the same
        ///     for an em square.)
        /// </remarks>
        public int EmSquare
        {
            get
            {
                EnsureHeadTable();
                return _head.UnitsPermEm;
            }
        }

        /// <summary>
        ///     Gets the main italic angle of the font expressed in tenths of
        ///     a degree counterclockwise from the vertical.
        /// </summary>
        /// <remarks>
        ///     Regular (roman) fonts have a value of zero. Italic fonts typically
        ///     have a negative italic angle (that is, they lean to the right).
        /// </remarks>
        public int ItalicAngle
        {
            get
            {
                EnsurePostTable();
                // TODO: Is the italic angle always a whole number?
                return _converter.ToPdfUnits( (int)_post.ItalicAngle );
            }
        }

        /// <summary>
        ///     Specifies the maximum distance characters in this font extend
        ///     above the base line. This is the typographic ascent for the font.
        /// </summary>
        public int Ascent
        {
            get
            {
                EnsureHheaTable();
                return _converter.ToPdfUnits( _hhea.Ascender );
            }
        }

        /// <summary>
        ///     Specifies the maximum distance characters in this font extend
        ///     below the base line. This is the typographic descent for the font.
        /// </summary>
        public int Descent
        {
            get
            {
                EnsureHheaTable();
                return _converter.ToPdfUnits( _hhea.Decender );
            }
        }

        /// <summary>
        ///     Gets the distance between the baseline and the approximate
        ///     height of uppercase letters.
        /// </summary>
        public int CapHeight
        {
            get
            {
                EnsureOs2Table();
                return _converter.ToPdfUnits( _os2.CapHeight );
            }
        }

        /// <summary>
        ///     Gets the distance between the baseline and the approximate
        ///     height of non-ascending lowercase letters.
        /// </summary>
        public int XHeight
        {
            get
            {
                EnsureOs2Table();
                return _converter.ToPdfUnits( _os2.XHeight );
            }
        }

        /// <summary>
        ///     TODO: The thickness, measured horizontally, of the dominant vertical
        ///     stems of the glyphs in the font.
        /// </summary>
        public int StemV
        {
            get
            {
                // TODO: Must be calculated somehow.
                return _converter.ToPdfUnits( 0 );
            }
        }

        /// <summary>
        ///     Gets the value of the first character defined in the font
        /// </summary>
        public ushort FirstChar
        {
            get
            {
                EnsureOs2Table();
                return _os2.FirstChar;
            }
        }

        /// <summary>
        ///     Gets the value of the last character defined in the font
        /// </summary>
        public ushort LastChar
        {
            get
            {
                EnsureOs2Table();
                return _os2.LastChar;
            }
        }

        /// <summary>
        ///     Gets the average width of glyphs in a font.
        /// </summary>
        public int AverageWidth
        {
            get
            {
                // TODO
                return 0;
            }
        }

        /// <summary>
        ///     Gets the maximum width of glyphs in a font.
        /// </summary>
        public int MaxWidth
        {
            get
            {
                // TODO: Could calculate from bounding box?
                return 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the font can be legally embedded
        ///     within a document.
        /// </summary>
        public bool IsEmbeddable
        {
            get
            {
                EnsureOs2Table();
                return _os2.IsEmbeddable;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the font can be legally subsetted.
        /// </summary>
        public bool IsSubsettable
        {
            get
            {
                EnsureOs2Table();
                return _os2.IsSubsettable;
            }
        }

        /// <summary>
        ///     Gets the font's bounding box.
        /// </summary>
        /// <remarks>
        ///     This is the smallest rectangle enclosing the shape that would
        ///     result if all the glyphs of the font were placed with their
        ///     origins cooincident and then filled.
        /// </remarks>
        public int[] BoundingBox
        {
            get
            {
                EnsureHeadTable();
                return new[]
                {
                    _converter.ToPdfUnits( _head.XMin ),
                    _converter.ToPdfUnits( _head.YMin ),
                    _converter.ToPdfUnits( _head.XMax ),
                    _converter.ToPdfUnits( _head.YMax )
                };
            }
        }

        /// <summary>
        ///     Gets a collection of flags defining various characteristics of
        ///     a font (e.g. serif or sans-serif, symbolic, etc).
        /// </summary>
        public int Flags
        {
            get
            {
                EnsureOs2Table();

                var flags = new BitVector32( 0 );
                flags[ 1 ] = _os2.IsMonospaced;
                flags[ 2 ] = _os2.IsSerif;
                flags[ 8 ] = _os2.IsScript;
                flags[ 64 ] = _os2.IsItalic;

                // Symbolic and NonSymbolic are mutually exclusive
                if ( _os2.IsSymbolic )
                    flags[ 4 ] = true;
                else
                    flags[ 32 ] = true;

                if ( flags.Data == 0 )
                {
                    // Nonsymbolic is a good default
                    return 32;
                }
                return flags.Data;
            }
        }

        /// <summary>
        ///     Gets a collection of kerning pairs.
        /// </summary>
        /// <returns></returns>
        public GdiKerningPairs KerningPairs
        {
            get
            {
                if ( _reader.ContainsTable( TableNames.Kern ) )
                {
                    _kern = (KerningTable)_reader.GetTable( TableNames.Kern );
                    return new GdiKerningPairs( _kern.KerningPairs, _converter );
                }
                return GdiKerningPairs.Empty;
            }
        }

        /// <summary>
        ///     Gets a collection of kerning pairs for characters defined in
        ///     the WinAnsiEncoding scheme only.
        /// </summary>
        /// <returns></returns>
        public GdiKerningPairs AnsiKerningPairs
        {
            get
            {
                if ( _reader.ContainsTable( TableNames.Kern ) )
                {
                    _kern = (KerningTable)_reader.GetTable( TableNames.Kern );

                    // The kerning pairs obtained from the TrueType font are keyed 
                    // on glyph index, whereas the ansi kerning pairs should be keyed 
                    // on codepoint value from the WinAnsiEncoding scheme.
                    KerningPairs oldPairs = _kern.KerningPairs;
                    var newPairs = new KerningPairs();

                    // Maps a unicode character to a codepoint value
                    WinAnsiMapping mapping = WinAnsiMapping.Mapping;

                    // TODO: Loop represents a cartesian product (256^2 = 65536)
                    for ( var i = 0; i < 256; i++ )
                    {
                        // Glyph index of character i
                        ushort leftIndex = _ranges.MapCharacter( (char)i );
                        for ( var j = 0; j < 256; j++ )
                        {
                            // Glyph index of character j
                            ushort rightIndex = _ranges.MapCharacter( (char)j );
                            if ( oldPairs.HasKerning( leftIndex, rightIndex ) )
                            {
                                // Create new kerning pair mapping codepoint pair 
                                // to kerning amount
                                newPairs.Add(
                                    mapping.MapCharacter( (char)i ),
                                    mapping.MapCharacter( (char)j ),
                                    oldPairs[ leftIndex, rightIndex ] );
                            }
                        }
                    }
                    return new GdiKerningPairs( newPairs, _converter );
                }
                return GdiKerningPairs.Empty;
            }
        }

        /// <summary>
        ///     Gets font metric data for a TrueType font or TrueType collection.
        /// </summary>
        /// <returns></returns>
        public byte[] GetFontData()
        {
            if ( _data == null )
            {
                try
                {
                    // Check if this is a TrueType font collection
                    uint ttcfTag = TableNames.ToUint( TableNames.Ttcf );
                    uint ttcfSize = LibWrapper.GetFontData( _dc.Handle, ttcfTag, 0, null, 0 );

                    if ( ttcfSize != 0 && ttcfSize != 0xFFFFFFFF )
                        _data = ReadFontFromCollection();
                    else
                        _data = ReadFont();
                }
                catch ( Exception e )
                {
                    throw new Exception(
                        string.Format( "Failed to load data for font {0}", FaceName ), e );
                }
            }

            return _data;
        }

        private byte[] ReadFontFromCollection()
        {
            var creator = new GdiFontCreator( _dc );
            return creator.Build();
        }

        private byte[] ReadFont()
        {
            uint bufferSize = LibWrapper.GetFontData( _dc.Handle, 0, 0, null, 0 );

            if ( bufferSize == 0xFFFFFFFF )
                throw new InvalidOperationException( "No font selected into device context" );

            var buffer = new byte[ bufferSize ];
            uint rv = LibWrapper.GetFontData( _dc.Handle, 0, 0, buffer, bufferSize );
            if ( rv == GdiError )
                throw new Exception( "Failed to retrieve table data for font " + FaceName );

            return buffer;
        }

        /// <summary>
        ///     Retrieves the widths, in PDF units, of consecutive glyphs.
        /// </summary>
        /// <returns>
        ///     An array of integers whose size is equal to the number of glyphs
        ///     specified in the 'maxp' table.
        ///     The width at location 0 is the width of glyph with index 0,
        ///     The width at location 1 is the width of glyph with index 1,
        ///     etc...
        /// </returns>
        public int[] GetWidths()
        {
            EnsureHmtxTable();

            var widths = new int[ _hmtx.Count ];

            // Convert each width to PDF units
            for ( var i = 0; i < _hmtx.Count; i++ )
                widths[ i ] = _converter.ToPdfUnits( _hmtx[ i ].AdvanceWidth );

            return widths;
        }

        /// <summary>
        ///     Returns the width, in PDF units, of consecutive glyphs for the
        ///     WinAnsiEncoding only.
        /// </summary>
        /// <returns>An array consisting of 256 elements.</returns>
        public int[] GetAnsiWidths()
        {
            EnsureHmtxTable();

            // WinAnsiEncoding consists of 256 characters
            var widths = new int[ 256 ];

            // The glyph at position 0 always represents the .notdef glyph
            int missingWidth = _converter.ToPdfUnits( _hmtx[ 0 ].AdvanceWidth );
            for ( var c = 0; c < 256; c++ )
                widths[ c ] = missingWidth;

            // Convert a unicode character to a code point value in the 
            // WinAnsiEncoding scheme
            WinAnsiMapping mapping = WinAnsiMapping.Mapping;

            for ( var c = 0; c < 256; c++ )
            {
                ushort glyphIndex = MapCharacter( (char)c );
                ushort codepoint = mapping.MapCharacter( (char)c );
                widths[ codepoint ] = _converter.ToPdfUnits( _hmtx[ glyphIndex ].AdvanceWidth );
            }

            return widths;
        }

        /// <summary>
        ///     Translates the supplied character to a glyph index using the
        ///     currently selected font.
        /// </summary>
        /// <param name="c">A unicode character.</param>
        /// <returns></returns>
        public ushort MapCharacter( char c )
        {
            return _ranges.MapCharacter( c );
        }

        private void EnsureHmtxTable()
        {
            if ( _hmtx == null )
                _hmtx = (HorizontalMetricsTable)GetTable( TableNames.Hmtx );
        }

        private void EnsureHheaTable()
        {
            if ( _hhea == null )
                _hhea = (HorizontalHeaderTable)GetTable( TableNames.Hhea );
        }

        private void EnsurePostTable()
        {
            if ( _post == null )
                _post = (PostTable)GetTable( TableNames.Post );
        }

        private void EnsureHeadTable()
        {
            if ( _head == null )
                _head = (HeaderTable)GetTable( TableNames.Head );
        }

        private void EnsureOs2Table()
        {
            if ( _os2 == null )
                _os2 = (Os2Table)GetTable( TableNames.Os2 );
        }

        private FontTable GetTable( string name )
        {
            try
            {
                return _reader.GetTable( name );
            }
            catch
            {
                throw new Exception( string.Format(
                    "Unable to retrieve table {0} from font {1}", name, FaceName ) );
            }
        }
    }
}