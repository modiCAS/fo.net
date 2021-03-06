using System.Collections;

namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the Index To Location ('loca') table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/loca.htm
    /// </remarks>
    internal class IndexToLocationTable : FontTable
    {
        private IList _offsets;

        /// <summary>
        ///     Initialises a new instance of the
        ///     <see cref="IndexToLocationTable" /> class.
        /// </summary>
        /// <param name="entry"></param>
        public IndexToLocationTable( DirectoryEntry entry )
            : base( TableNames.Loca, entry )
        {
        }

        /// <summary>
        ///     Initialises a new instance of the IndexToLocationTable class.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="numOffsets"></param>
        public IndexToLocationTable( DirectoryEntry entry, int numOffsets )
            : base( TableNames.Loca, entry )
        {
            _offsets = new ArrayList( numOffsets );
        }

        /// <summary>
        ///     Gets the number of glyph offsets.
        /// </summary>
        public int Count
        {
            get { return _offsets.Count; }
        }

        /// <summary>
        ///     Gets or sets the glyph offset at index <i>index</i>.
        /// </summary>
        /// <param name="index">A glyph index.</param>
        /// <returns></returns>
        public uint this[ int index ]
        {
            get { return (uint)_offsets[ index ]; }
            set { _offsets.Insert( index, value ); }
        }

        /// <summary>
        ///     Reads the contents of the "loca" table from the supplied stream
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;

            // Glyph offsets can be stored in either short of long format
            bool isShortFormat = reader.GetHeaderTable().IsShortFormat;

            // Number of glyphs including extra entry
            int glyphCount = reader.GetMaximumProfileTable().GlyphCount + 1;

            _offsets = new ArrayList( glyphCount );
            for ( var i = 0; i < glyphCount; i++ )
                _offsets.Insert( i, isShortFormat ? (uint)( stream.ReadUShort() << 1 ) : stream.ReadULong() );
        }

        protected internal override void Write( FontFileWriter writer )
        {
            // TODO: Determine short/long format
            foreach ( uint offset in _offsets )
                writer.Stream.WriteULong( offset );
        }

        /// <summary>
        ///     Removes all offsets.
        /// </summary>
        public void Clear()
        {
            _offsets.Clear();
        }

        /// <summary>
        ///     Includes the supplied offset.
        /// </summary>
        /// <param name="offset"></param>
        public void AddOffset( uint offset )
        {
            _offsets.Add( offset );
        }
    }
}