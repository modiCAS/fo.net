using System;
using System.IO;
using Fonet.Pdf.Gdi.Font;
using Fonet.Pdf.Gdi.Font.Tables;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     Retrieves all pertinent TrueType tables by invoking GetFontData.
    /// </summary>
    public class GdiFontCreator
    {
        private const int NumTables = 11;

        private readonly GdiDeviceContent _dc;
        private readonly FontFileStream _fs;
        private TrueTypeHeader _header;
        private readonly MemoryStream _ms;

        private int _offset;

        public GdiFontCreator( GdiDeviceContent dc )
        {
            this._dc = dc;
            _header = new TrueTypeHeader();
            _ms = new MemoryStream();
            _fs = new FontFileStream( _ms );
        }

        public byte[] Build()
        {
            byte[] headData = ReadTableData( TableNames.Head );
            byte[] maxpData = ReadTableData( TableNames.Maxp );
            byte[] hheaData = ReadTableData( TableNames.Hhea );
            byte[] hmtxData = ReadTableData( TableNames.Hmtx );
            byte[] cvtData = ReadTableData( TableNames.Cvt );
            byte[] prepData = ReadTableData( TableNames.Prep );
            byte[] fpgmData = ReadTableData( TableNames.Fpgm );
            byte[] glyfData = ReadTableData( TableNames.Glyf );
            byte[] locaData = ReadTableData( TableNames.Loca );
            byte[] os2Data = ReadTableData( TableNames.Os2 );
            byte[] postData = ReadTableData( TableNames.Post );

            // Write TrueType header
            _fs.WriteFixed( 0x00010000 ); // sfnt Version
            _fs.WriteUShort( 11 );
            _fs.WriteUShort( 0 ); // search range
            _fs.WriteUShort( 0 ); // entry selector
            _fs.WriteUShort( 0 ); // range shift

            // Offsets begin from end of table directory
            _offset = (int)_fs.Position + NumTables * PrimitiveSizes.ULong * 4;

            // Write directory entry for each table
            WriteDirectoryEntry( TableNames.Head, headData );
            WriteDirectoryEntry( TableNames.Maxp, maxpData );
            WriteDirectoryEntry( TableNames.Hhea, hheaData );
            WriteDirectoryEntry( TableNames.Hmtx, hmtxData );
            WriteDirectoryEntry( TableNames.Cvt, cvtData );
            WriteDirectoryEntry( TableNames.Prep, prepData );
            WriteDirectoryEntry( TableNames.Fpgm, fpgmData );
            WriteDirectoryEntry( TableNames.Glyf, glyfData );
            WriteDirectoryEntry( TableNames.Loca, locaData );
            WriteDirectoryEntry( TableNames.Os2, os2Data );
            WriteDirectoryEntry( TableNames.Post, postData );

            _fs.Write( headData, 0, headData.Length );
            _fs.Write( maxpData, 0, maxpData.Length );
            _fs.Write( hheaData, 0, hheaData.Length );
            _fs.Write( hmtxData, 0, hmtxData.Length );
            _fs.Write( cvtData, 0, cvtData.Length );
            _fs.Write( prepData, 0, prepData.Length );
            _fs.Write( fpgmData, 0, fpgmData.Length );
            _fs.Write( glyfData, 0, glyfData.Length );
            _fs.Write( locaData, 0, locaData.Length );
            _fs.Write( os2Data, 0, os2Data.Length );
            _fs.Write( postData, 0, postData.Length );

            return _ms.ToArray();
        }

        private void WriteTable( byte[] data )
        {
            _fs.Write( data, 0, data.Length );

            // Align table on 4-byte boundary
            _fs.Pad();
        }

        private void WriteDirectoryEntry( string tableName, byte[] data )
        {
            _fs.WriteByte( (byte)tableName[ 0 ] );
            _fs.WriteByte( (byte)tableName[ 1 ] );
            _fs.WriteByte( (byte)tableName[ 2 ] );
            _fs.WriteByte( (byte)tableName[ 3 ] );
            _fs.WriteULong( 0 );
            _fs.WriteULong( (uint)_offset );
            _fs.WriteULong( (uint)data.Length );

            _offset += data.Length;
        }

        private byte[] ReadTableData( string tableName )
        {
            uint tag = TableNames.ToUint( tableName );
            uint size = LibWrapper.GetFontData( _dc.Handle, tag, 0, null, 0 );

            var data = new byte[ size ];
            uint rv = LibWrapper.GetFontData( _dc.Handle, tag, 0, data, (uint)data.Length );
            if ( rv == GdiFontMetrics.GdiError )
                throw new Exception( "Failed to retrieve table " + tableName );

            return data;
        }
    }
}