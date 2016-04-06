using System;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Class that represents the Horizontal Header table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/hhea.htm
    /// </remarks>
    internal class HorizontalHeaderTable : FontTable
    {
        /// <summary>
        ///     Maximum advance width value in 'hmtx' table.
        /// </summary>
        internal ushort AdvanceWidthMax;

        /// <summary>
        ///     Typographic ascent. (Distance from baseline of highest ascender).
        /// </summary>
        internal short Ascender;

        /// <summary>
        ///     The amount by which a slanted highlight on a glyph needs to be
        ///     shifted to produce the best appearance. Set to 0 for non-slanted fonts.
        /// </summary>
        internal short CaretOffset;

        /// <summary>
        ///     Used to calculate the slope of the cursor (rise/run); 1 for vertical.
        /// </summary>
        internal short CaretSlopeRise;

        /// <summary>
        ///     0 for vertical.
        /// </summary>
        internal short CaretSlopeRun;

        /// <summary>
        ///     Typographic descent. (Distance from baseline of lowest descender).
        /// </summary>
        internal short Decender;

        /// <summary>
        ///     Typographic line gap.  Negative LineGap values are treated as zero
        ///     in Windows 3.1, System 6, and System 7.
        /// </summary>
        internal short LineGap;

        /// <summary>
        ///     0 for current format.
        /// </summary>
        internal short MetricDataFormat;

        /// <summary>
        ///     Minimum left sidebearing value in 'hmtx' table.
        /// </summary>
        internal short MinLeftSideBearing;

        /// <summary>
        ///     Minimum right sidebearing value.
        /// </summary>
        internal short MinRightSideBearing;

        /// <summary>
        ///     Number of hMetric entries in 'hmtx' table.
        /// </summary>
        internal ushort NumberOfHMetrics;

        /// <summary>
        ///     Table version number 0x00010000 for version 1.0.
        /// </summary>
        internal int VersionNo;

        /// <summary>
        ///     Max(lsb + (xMax - xMin)).
        /// </summary>
        internal short XMaxExtent;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public HorizontalHeaderTable( DirectoryEntry entry ) : base( TableNames.Hhea, entry )
        {
        }

        /// <summary>
        ///     Gets the number of horiztonal metrics.
        /// </summary>
        public int HMetricCount
        {
            get { return NumberOfHMetrics; }
            set { NumberOfHMetrics = Convert.ToUInt16( value ); }
        }

        /// <summary>
        ///     Reads the contents of the "hhea" table from the current position
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;
            VersionNo = stream.ReadFixed();
            Ascender = stream.ReadFWord();
            Decender = stream.ReadFWord();
            LineGap = stream.ReadFWord();
            AdvanceWidthMax = stream.ReadUfWord();
            MinLeftSideBearing = stream.ReadFWord();
            MinRightSideBearing = stream.ReadFWord();
            XMaxExtent = stream.ReadFWord();
            CaretSlopeRise = stream.ReadShort();
            CaretSlopeRun = stream.ReadShort();
            CaretOffset = stream.ReadShort();
            // TODO: Check these 4 fields are all 0
            stream.ReadShort();
            stream.ReadShort();
            stream.ReadShort();
            stream.ReadShort();
            MetricDataFormat = stream.ReadShort();
            NumberOfHMetrics = stream.ReadUShort();
        }

        protected internal override void Write( FontFileWriter writer )
        {
            FontFileStream stream = writer.Stream;

            stream.WriteFixed( VersionNo );
            stream.WriteFWord( Ascender );
            stream.WriteFWord( Decender );
            stream.WriteFWord( LineGap );
            stream.WriteUfWord( AdvanceWidthMax );
            stream.WriteFWord( MinLeftSideBearing );
            stream.WriteFWord( MinRightSideBearing );
            stream.WriteFWord( XMaxExtent );
            stream.WriteShort( CaretSlopeRise );
            stream.WriteShort( CaretSlopeRun );
            stream.WriteShort( CaretOffset );
            // TODO: Check these 4 fields are all 0
            stream.WriteShort( 0 );
            stream.WriteShort( 0 );
            stream.WriteShort( 0 );
            stream.WriteShort( 0 );
            stream.WriteShort( MetricDataFormat );
            stream.WriteUShort( NumberOfHMetrics );
        }
    }
}