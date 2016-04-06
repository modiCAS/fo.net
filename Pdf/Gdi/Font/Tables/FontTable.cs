using System;

namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class derived by all TrueType table classes.
    /// </summary>
    internal abstract class FontTable
    {
        /// <summary>
        ///     The dictionary entry for this table.
        /// </summary>
        private DirectoryEntry _directoryEntry;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="entry">Table directory entry.</param>
        protected FontTable( string tableName, DirectoryEntry entry )
        {
            _directoryEntry = entry;
        }

        /// <summary>
        ///     Gets or sets a directory entry for this table.
        /// </summary>
        public DirectoryEntry Entry
        {
            get { return _directoryEntry; }
            set { _directoryEntry = value; }
        }

        /// <summary>
        ///     Gets the unique name of this table as a 4-character string.
        /// </summary>
        /// <remarks>
        ///     Note that some TrueType tables are only 3 characters long
        ///     (e.g. 'cvt').  In this case the returned string will be padded
        ///     with a extra space at the end of the string.
        /// </remarks>
        public string Name
        {
            get { return _directoryEntry.TableName; }
        }

        /// <summary>
        ///     Gets the table name encoded as a 32-bit unsigned integer.
        /// </summary>
        public uint Tag
        {
            get { return _directoryEntry.Tag; }
        }

        /// <summary>
        ///     Reads the contents of a table from the current position in
        ///     the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="ArgumentException">
        ///     If the supplied stream does not contain enough data.
        /// </exception>
        protected internal abstract void Read( FontFileReader reader );

        /// <summary>
        ///     Writes the contents of a table to the supplied writer.
        /// </summary>
        /// <remarks>
        ///     This method should not be concerned with aligning the
        ///     table output on the 4-byte boundary.
        /// </remarks>
        /// <param name="writer"></param>
        protected internal abstract void Write( FontFileWriter writer );
    }
}