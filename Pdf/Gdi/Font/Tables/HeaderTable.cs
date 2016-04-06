using System;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Class that represents the Font Header table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/head.htm
    /// </remarks>
    internal class HeaderTable : FontTable
    {
        private static readonly DateTime BaseDate =
            new DateTime( 1904, 1, 1, 0, 0, 0 );

        internal uint CheckSumAdjustment;
        internal DateTime CreateDate;
        internal ushort Flags;
        internal short FontDirectionHint;
        internal int FontRevision;
        internal short GlyphDataFormat;
        internal short IndexToLocFormat;
        internal ushort LowestRecPpem;
        internal ushort MacStyle;
        internal uint MagicNumber;
        internal ushort UnitsPermEm;
        internal DateTime UpdateDate;
        internal int VersionNo;
        internal short XMax;
        internal short XMin;
        internal short YMax;
        internal short YMin;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public HeaderTable( DirectoryEntry entry )
            : base( TableNames.Head, entry )
        {
        }

        /// <summary>
        ///     Gets a value that indicates whether glyph offsets in the
        ///     loca table are stored as a ushort or ulong.
        /// </summary>
        public bool IsShortFormat
        {
            get { return IndexToLocFormat == 0; }
        }

        /// <summary>
        ///     Reads the contents of the "head" table from the current position
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;
            VersionNo = stream.ReadFixed();
            FontRevision = stream.ReadFixed();
            CheckSumAdjustment = stream.ReadULong();
            MagicNumber = stream.ReadULong();
            Flags = stream.ReadUShort();
            UnitsPermEm = stream.ReadUShort();
            // Some fonts have dodgy date offsets that cause AddSeconds to throw an exception
            CreateDate = GetDate( stream.ReadLongDateTime() );
            UpdateDate = GetDate( stream.ReadLongDateTime() );
            XMin = stream.ReadShort();
            YMin = stream.ReadShort();
            XMax = stream.ReadShort();
            YMax = stream.ReadShort();
            MacStyle = stream.ReadUShort();
            LowestRecPpem = stream.ReadUShort();
            FontDirectionHint = stream.ReadShort();
            IndexToLocFormat = stream.ReadShort();
            GlyphDataFormat = stream.ReadShort();
        }

        /// <summary>
        ///     Returns a DateTime instance which is the result of adding <i>seconds</i>
        ///     to BaseDate.  If an exception occurs, BaseDate is returned.
        /// </summary>
        /// <param name="seconds"></param>
        private DateTime GetDate( long seconds )
        {
            try
            {
                return new DateTime( BaseDate.Ticks ).AddSeconds( seconds );
            }
            catch
            {
                return BaseDate;
            }
        }

        /// <summary>
        ///     Writes the contents of the head table to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write( FontFileWriter writer )
        {
            FontFileStream stream = writer.Stream;
            stream.WriteFixed( VersionNo );
            stream.WriteFixed( FontRevision );
            // TODO: Calculate based on entire font 
            stream.WriteULong( 0 );
            stream.WriteULong( 0x5F0F3CF5 );
            stream.WriteUShort( Flags );
            stream.WriteUShort( UnitsPermEm );
            stream.WriteDateTime( (long)( CreateDate - BaseDate ).TotalSeconds );
            stream.WriteDateTime( (long)( UpdateDate - BaseDate ).TotalSeconds );
            stream.WriteShort( XMin );
            stream.WriteShort( YMin );
            stream.WriteShort( XMax );
            stream.WriteShort( YMax );
            stream.WriteUShort( MacStyle );
            stream.WriteUShort( LowestRecPpem );
            stream.WriteShort( FontDirectionHint );
            // TODO: Always write loca offsets as ulongs
            stream.WriteShort( 1 );
            stream.WriteShort( GlyphDataFormat );
        }
    }
}