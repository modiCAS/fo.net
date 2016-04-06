using System.Collections;

namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the Horizontal Metrics ('hmtx') table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/hmtx.htm
    /// </remarks>
    internal class HorizontalMetricsTable : FontTable
    {
        public IList Metrics;

        /// <summary>
        ///     Initialises a new instance of the
        ///     <see cref="HorizontalMetricsTable" /> class.
        /// </summary>
        /// <param name="entry"></param>
        public HorizontalMetricsTable( DirectoryEntry entry )
            : base( TableNames.Hmtx, entry )
        {
        }

        /// <summary>
        ///     Initialises a new instance of the HorizontalMetricsTable class.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="numMetrics"></param>
        public HorizontalMetricsTable( DirectoryEntry entry, int numMetrics )
            : base( TableNames.Hmtx, entry )
        {
            Metrics = new ArrayList( numMetrics );
        }

        /// <summary>
        ///     Returns the number of horizontal metrics stored in the
        ///     hmtx table.
        /// </summary>
        public int Count
        {
            get { return Metrics.Count; }
        }

        /// <summary>
        ///     Gets the <see cref="HorizontalMetric" /> located at <i>index</i>.
        /// </summary>
        public HorizontalMetric this[ int index ]
        {
            get { return (HorizontalMetric)Metrics[ index ]; }
            set { Metrics.Insert( index, value ); }
        }

        /// <summary>
        ///     Reads the contents of the "hmtx" table from the supplied stream
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;

            // Obtain number of horizonal metrics from 'hhea' table
            int numberOfHMetrics = reader.GetHorizontalHeaderTable().HMetricCount;

            // Obtain glyph count from 'maxp' table
            int numGlyphs = reader.GetMaximumProfileTable().GlyphCount;

            // Metrics might not be supplied for each glyph.  For example, if 
            // the font is monospaced the hMetrics array will only contain a 
            // single entry
            int metricsSize = numGlyphs > numberOfHMetrics ? numGlyphs : numberOfHMetrics;
            Metrics = new ArrayList( metricsSize );

            for ( var i = 0; i < numberOfHMetrics; i++ )
            {
                Metrics.Add( new HorizontalMetric(
                    stream.ReadUShort(), stream.ReadShort() ) );
            }

            // Fill in missing widths
            if ( numberOfHMetrics < metricsSize )
            {
                var lastHMetric = (HorizontalMetric)Metrics[ Metrics.Count - 1 ];
                for ( int i = numberOfHMetrics; i < numGlyphs; i++ )
                {
                    Metrics.Add(
                        new HorizontalMetric( lastHMetric.AdvanceWidth, stream.ReadShort() ) );
                }
            }
        }

        protected internal override void Write( FontFileWriter writer )
        {
            FontFileStream stream = writer.Stream;
            for ( var i = 0; i < Metrics.Count; i++ )
            {
                var metric = (HorizontalMetric)Metrics[ i ];
                stream.WriteUShort( metric.AdvanceWidth );
                stream.WriteShort( metric.LeftSideBearing );
            }
        }
    }
}