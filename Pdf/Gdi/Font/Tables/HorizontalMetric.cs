namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Summary description for HorizontalMetric.
    /// </summary>
    internal class HorizontalMetric
    {
        public HorizontalMetric( ushort advanceWidth, short leftSideBearing )
        {
            AdvanceWidth = advanceWidth;
            LeftSideBearing = leftSideBearing;
        }

        public ushort AdvanceWidth { get; private set; }

        public short LeftSideBearing { get; private set; }

        public HorizontalMetric Clone()
        {
            return new HorizontalMetric( AdvanceWidth, LeftSideBearing );
        }
    }
}