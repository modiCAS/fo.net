using System;
using System.Text;
using Fonet.Pdf.Gdi.Font.Tables;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Represents an entry in the directory table
    /// </summary>
    internal class DirectoryEntry
    {
        public DirectoryEntry( string tagName )
        {
            Tag =
                (uint)
                    ( ( (byte)tagName[ 0 ] << 24 ) | ( (byte)tagName[ 1 ] << 16 ) | ( (byte)tagName[ 2 ] << 8 ) |
                        (byte)tagName[ 3 ] );
            TableName = tagName;
        }

        public DirectoryEntry( byte[] tag, uint checkSum, uint offset, uint length )
        {
            if ( tag == null )
                throw new ArgumentNullException( "tag", "tag cannot be null" );
            if ( tag.Length != 4 )
                throw new ArgumentException( "tag array must be 4 bytes in size", "tag" );

            Tag = (uint)( ( tag[ 0 ] << 24 ) | ( tag[ 1 ] << 16 ) | ( tag[ 2 ] << 8 ) | tag[ 3 ] );
            TableName = Encoding.ASCII.GetString( tag );
            CheckSum = checkSum;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        ///     Returns the table tag as a string
        /// </summary>
        /// <returns></returns>
        public string TableName { get; private set; }

        /// <summary>
        ///     Gets the table tag encoded as an unsigned 32-bite integer.
        /// </summary>
        public uint Tag { get; private set; }

        /// <summary>
        ///     Gets or sets a value that represents a <see cref="FontTable" />
        ///     offset, i.e. the number of bytes from the beginning of the file.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        ///     Gets or sets a value representing the number number of bytes
        ///     a <see cref="FontTable" /> object occupies in a stream.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        ///     Gets or sets value that represents a checksum of a <see cref="FontTable" />.
        /// </summary>
        public uint CheckSum { get; set; }

        /// <summary>
        ///     Gets an instance of an <see cref="FontTable" /> implementation that is
        ///     capable of parsing the table identified by <b>tab</b>.
        /// </summary>
        /// <returns></returns>
        internal FontTable MakeTable( FontFileReader reader )
        {
            return FontTableFactory.Make( TableName, reader );
        }
    }
}