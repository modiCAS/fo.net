using System;
using System.Collections;
using System.IO;
using Fonet.Pdf.Gdi.Font.Tables;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     A specialised stream writer for creating OpenType fonts.
    /// </summary>
    internal class FontFileWriter
    {
        /// <summary>
        ///     Size of the offset table in bytes.
        /// </summary>
        private const int OffsetTableSize = PrimitiveSizes.Fixed + 4 * PrimitiveSizes.UShort;

        /// <summary>
        ///     The underlying stream.
        /// </summary>
        private readonly FontFileStream _stream;

        /// <summary>
        ///     List of font tables to write.
        /// </summary>
        private readonly IDictionary _tables;

        /// <summary>
        ///     Creates a new instance of the <see cref="FontFileWriter" /> class
        ///     using <i>stream</i> as the underlying stream object.
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentException">
        ///     If <i>stream</i> is not writable.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <i>streamm</i> is a null reference.
        /// </exception>
        public FontFileWriter( Stream stream )
        {
            if ( stream == null )
                throw new ArgumentNullException( "stream", "Supplied stream cannot be a null reference" );
            if ( !stream.CanWrite )
                throw new ArgumentException( "The supplied stream is not writable", "stream" );
            this._stream = new FontFileStream( stream );
            _tables = new Hashtable();
        }

        /// <summary>
        ///     Gets the underlying <see cref="FontFileStream" />.
        /// </summary>
        public FontFileStream Stream
        {
            get { return _stream; }
        }

        /// <summary>
        ///     Queues the supplied <see cref="FontTable" /> for writing
        ///     to the underlying stream.
        /// </summary>
        /// <remarks>
        ///     The method will not immediately write the supplied font
        ///     table to the underlying stream.  Instead it queues the
        ///     font table since the offset table must be written out
        ///     before any tables.
        /// </remarks>
        /// <param name="table"></param>
        public void Write( FontTable table )
        {
            if ( _tables.Contains( table.Name ) )
                throw new ArgumentException( "Already written table '" + table.Name + "'" );
            _tables.Add( table.Name, table );
        }

        /// <summary>
        ///     Writes the header and font tables to the underlying stream.
        /// </summary>
        public void Close()
        {
            WriteOffsetTable();
            SkipTableDirectory();
            WriteTables();
            WriteTableDirectory();
            WriteChecksumAdjustment();
        }

        /// <summary>
        ///     Updates the checkSumAdjustment field in the head table.
        /// </summary>
        private void WriteChecksumAdjustment()
        {
            var head = (HeaderTable)_tables[ TableNames.Head ];

            // Move to beginning of head table and skip the version no and 
            // font revision no fields.
            _stream.Position = head.Entry.Offset + 2 * PrimitiveSizes.Fixed;
            _stream.WriteULong( CalculateCheckSumAdjustment() );
        }

        /// <summary>
        ///     Writes out each table to the font stream.
        /// </summary>
        private void WriteTables()
        {
            foreach ( FontTable table in _tables.Values )
                WriteFontTable( table );
        }

        private void WriteFontTable( FontTable table )
        {
            // Start position required to generate checksum and length
            long startPosition = _stream.SetRestorePoint();

            // FontTable subclass is responsible for writing itself
            table.Write( this );

            // Align table on 4-byte boundary
            int padding = _stream.Pad();

            // Restore will reset stream position back to startPosition
            long endPosition = _stream.Restore();

            // The table length not including the padding
            table.Entry.Length = (uint)( endPosition - startPosition - padding );
            table.Entry.Offset = (uint)startPosition;
            table.Entry.CheckSum = CalculateCheckSum( table.Entry.Length );
        }

        /// <summary>
        ///     Writes the offset table that appears at the beginning of
        ///     every TrueType/OpenType font.
        /// </summary>
        private void WriteOffsetTable()
        {
            // sfnt version (0x00010000 for version 1.0).
            _stream.WriteFixed( 0x00010000 );

            // numTables field
            int numTables = _tables.Count;
            _stream.WriteUShort( numTables );

            // searchRange field ((Maximum power of 2 <= numTables) x 16)
            int maxPower = MaxPow2( numTables );
            int searchRange = maxPower * 16;
            _stream.WriteUShort( searchRange );

            // entrySelector (Log2(maximum power of 2 <= numTables))
            var entrySelector = (int)Math.Log( maxPower, 2 );
            _stream.WriteUShort( entrySelector );

            // NumTables x 16-searchRange.
            int rangeShift = numTables * 16 - searchRange;
            _stream.WriteUShort( rangeShift );
        }

        private void WriteTableDirectory()
        {
            _stream.SetRestorePoint();
            _stream.Position = 0;
            _stream.Skip( OffsetTableSize );

            foreach ( FontTable table in _tables.Values )
            {
                _stream.WriteULong( table.Tag );
                _stream.WriteULong( table.Entry.CheckSum );
                _stream.WriteULong( table.Entry.Offset );
                _stream.WriteULong( table.Entry.Length );
            }

            _stream.Restore();
        }

        /// <summary>
        ///     Does not actually write the table directory - simply "allocates"
        ///     space for it in the stream.
        /// </summary>
        private void SkipTableDirectory()
        {
            _stream.Skip( _tables.Count * PrimitiveSizes.ULong * 4 );
        }

        /// <summary>
        ///     Returns the maximum power of 2 &lt;= max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private int MaxPow2( int max )
        {
            var i = 0;
            while ( Math.Pow( 2, i ) < max )
                i++;

            return i == 0 ? 0 : i - 1;
        }

        /// <summary>
        ///     Calculates the checksum of the entire font.
        /// </summary>
        /// <remarks>
        ///     The underlying <see cref="FontFileStream" /> must be aligned on
        ///     a 4-byte boundary.
        /// </remarks>
        /// <returns></returns>
        private uint CalculateCheckSumAdjustment()
        {
            long length = _stream.SetRestorePoint();

            _stream.Position = 0L;
            uint checkSum = 0xB1B0AFBA - CalculateCheckSum( length );
            _stream.Restore();

            return checkSum;
        }

        /// <summary>
        ///     Calculates the checksum of a <see cref="FontTable" />.
        /// </summary>
        /// <remarks>
        ///     The supplied <i>stream</i> must be positioned at the beginning of
        ///     the table.
        /// </remarks>
        /// <param name="length"></param>
        /// <returns></returns>
        private uint CalculateCheckSum( long length )
        {
            long numBytes = length + length % 4;
            uint checkSum = 0;
            for ( long i = 0; i < numBytes; i += PrimitiveSizes.ULong )
            {
                checkSum += _stream.ReadULong();
                if ( checkSum > 0xFFFFFFFF )
                    checkSum = checkSum - 0xFFFFFFFF;
            }

            return checkSum;
        }
    }
}