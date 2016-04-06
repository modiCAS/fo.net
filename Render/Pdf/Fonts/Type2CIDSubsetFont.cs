using System;
using System.IO;
using Fonet.Pdf;
using Fonet.Pdf.Gdi.Font;

namespace Fonet.Render.Pdf.Fonts
{
    /// <summary>
    ///     A subclass of Type2CIDFont that generates a subset of a
    ///     TrueType font.
    /// </summary>
    internal class Type2CidSubsetFont : Type2CidFont
    {
        /// <summary>
        ///     Maps a glyph index to a subset index.
        /// </summary>
        protected IndexMappings IndexMappings;

        /// <summary>
        ///     Quasi-unique six character name prefix.
        /// </summary>
        protected string NamePrefix;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        public Type2CidSubsetFont( FontProperties properties )
            : base( properties )
        {
            InsertNotdefGlyphs();
            NamePrefix = new Random().Next( 0x100000, 0xFFFFFF ).ToString( "X" ).Substring( 0, 6 );
        }

        public override PdfWArray WArray
        {
            get
            {
                // Allocate space for glyph subset
                var subsetWidths = new int[ IndexMappings.Count ];

                // Subset indices are returned in ascending order
                foreach ( int subsetIndex in IndexMappings.SubsetIndices )
                {
                    int glyphIndex = IndexMappings.GetGlyphIndex( subsetIndex );
                    subsetWidths[ subsetIndex ] = WidthArray[ glyphIndex ];
                }

                var widthsArray = new PdfWArray( 0 );
                widthsArray.AddEntry( subsetWidths );

                return widthsArray;
            }
        }

        public override string FontName
        {
            get { return string.Format( "{0}+{1}", NamePrefix, BaseFontName ); }
        }

        public override byte[] FontData
        {
            get
            {
                var input = new MemoryStream( Metrics.GetFontData() );
                var reader = new FontFileReader( input, Metrics.FaceName );
                reader.IndexMappings = IndexMappings;
                var subset = new FontSubset( reader );

                input = null;

                var output = new MemoryStream();
                subset.Generate( output );

                return output.GetBuffer();
            }
        }

        /// <summary>
        ///     Creates the index mappings list and adds the .notedef glyphs
        /// </summary>
        private void InsertNotdefGlyphs()
        {
            // Ensure first three glyph are included.  These glyphs represent 
            // the .notdef characters which is commonly located at glyph index 0, 
            // but sometimes indices 1 or 2.
            IndexMappings = new IndexMappings();
            IndexMappings.Add( 0, 1, 2 );
        }

        public override ushort MapCharacter( char c )
        {
            return (ushort)IndexMappings.GetSubsetIndex( base.MapCharacter( c ) );
        }

        protected override void AddGlyphToCharMapping( ushort glyphIndex, char c )
        {
            int subsetIndex = -1;
            if ( IndexMappings.HasMapping( glyphIndex ) )
                subsetIndex = IndexMappings.GetSubsetIndex( glyphIndex );
            else
            {
                // Generate new mapping
                subsetIndex = IndexMappings.Map( glyphIndex );
            }

            // The usedGlyphs dictionary permits a reverse lookup (glyph index to char)
            if ( !UsedGlyphs.ContainsKey( subsetIndex ) )
                UsedGlyphs.Add( subsetIndex, c );
        }

        public override int GetWidth( ushort charIndex )
        {
            int glyphIndex = IndexMappings.GetGlyphIndex( charIndex );

            // The widths array is keyed on character code, not glyph index
            return base.GetWidth( (ushort)glyphIndex );
        }
    }
}