using System.IO;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Generates a subset from a TrueType font.
    /// </summary>
    public class FontSubset
    {
        private readonly FontFileReader reader;

        /// <summary>
        ///     Creates a new instance of the FontSubset class.
        /// </summary>
        /// <param name="reader">TrueType font parser.</param>
        public FontSubset( FontFileReader reader )
        {
            this.reader = reader;
        }

        /// <summary>
        ///     Writes the font subset to the supplied output stream.
        /// </summary>
        public void Generate( MemoryStream output )
        {
            HeaderTable head = reader.GetHeaderTable();
            MaximumProfileTable maxp = reader.GetMaximumProfileTable();
            HorizontalHeaderTable hhea = reader.GetHorizontalHeaderTable();
            ControlValueTable cvt = reader.GetControlValueTable();
            FontProgramTable fpgm = reader.GetFontProgramTable();
            GlyfDataTable glyf = reader.GetGlyfDataTable();
            ControlValueProgramTable prep = reader.GetControlValueProgramTable();
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
            HorizontalMetricsTable hmtxOld = reader.GetHorizontalMetricsTable();

            var entry = new DirectoryEntry( TableNames.Hmtx );
            var hmtx =
                new HorizontalMetricsTable( entry, glyfTable.Count );

            // Copy required horizontal metrics.
            IndexMappings mappings = reader.IndexMappings;
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