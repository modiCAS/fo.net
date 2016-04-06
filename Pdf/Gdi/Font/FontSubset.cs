using System.IO;
using Fonet.Pdf.Gdi.Font.Tables;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Generates a subset from a TrueType font.
    /// </summary>
    public class FontSubset
    {
        private readonly FontFileReader _reader;

        /// <summary>
        ///     Creates a new instance of the FontSubset class.
        /// </summary>
        /// <param name="reader">TrueType font parser.</param>
        public FontSubset( FontFileReader reader )
        {
            this._reader = reader;
        }

        /// <summary>
        ///     Writes the font subset to the supplied output stream.
        /// </summary>
        public void Generate( MemoryStream output )
        {
            HeaderTable head = _reader.GetHeaderTable();
            MaximumProfileTable maxp = _reader.GetMaximumProfileTable();
            HorizontalHeaderTable hhea = _reader.GetHorizontalHeaderTable();
            ControlValueTable cvt = _reader.GetControlValueTable();
            FontProgramTable fpgm = _reader.GetFontProgramTable();
            GlyfDataTable glyf = _reader.GetGlyfDataTable();
            ControlValueProgramTable prep = _reader.GetControlValueProgramTable();
            IndexToLocationTable loca = CreateLocaTable( glyf );
            HorizontalMetricsTable hmtx = CreateHmtxTable( glyf );

            // Since we're reusing the hhea and maxp tables, we must update 
            // the numberOfHMetrics and numGlyphs fields to reflect the reduced 
            // number of glyphs.
            maxp.GlyphCount = glyf.Count;
            hhea.HMetricCount = glyf.Count;

            var writer = new FontFileWriter( output );
            writer.Write( head );
            writer.Write( maxp );
            writer.Write( hhea );
            writer.Write( hmtx );
            writer.Write( cvt );
            writer.Write( prep );
            writer.Write( fpgm );
            writer.Write( loca );
            writer.Write( glyf );
            writer.Close();
        }

        private HorizontalMetricsTable CreateHmtxTable( GlyfDataTable glyfTable )
        {
            // Fetches horizontal metrics for all glyphs
            HorizontalMetricsTable hmtxOld = _reader.GetHorizontalMetricsTable();

            var entry = new DirectoryEntry( TableNames.Hmtx );
            var hmtx =
                new HorizontalMetricsTable( entry, glyfTable.Count );

            // Copy required horizontal metrics.
            IndexMappings mappings = _reader.IndexMappings;
            foreach ( int subsetIndex in mappings.SubsetIndices )
            {
                int glyphIndex = mappings.GetGlyphIndex( subsetIndex );
                hmtx[ subsetIndex ] = hmtxOld[ glyphIndex ].Clone();
            }

            return hmtx;
        }

        private IndexToLocationTable CreateLocaTable( GlyfDataTable glyfTable )
        {
            var entry = new DirectoryEntry( TableNames.Loca );
            var loca = new IndexToLocationTable( entry, glyfTable.Count );

            uint offset = 0;
            for ( var i = 0; i < glyfTable.Count; i++ )
            {
                loca.AddOffset( offset );
                offset += glyfTable[ i ].Length;
            }

            // Add trailer offset
            loca.AddOffset( offset );

            return loca;
        }
    }
}