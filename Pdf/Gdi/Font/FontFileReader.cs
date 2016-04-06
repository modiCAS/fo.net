using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Class designed to parse a TrueType font file.
    /// </summary>
    public class FontFileReader
    {
        /// <summary>
        ///     Used to identity a font within a TrueType collection.
        /// </summary>
        private readonly string _fontName;

        /// <summary>
        ///     Maps a table name (4-character string) to a <see cref="DirectoryEntry" />
        /// </summary>
        private TrueTypeHeader _header;

        /// <summary>
        ///     Maps a glyph index to a subset index.
        /// </summary>
        private IndexMappings _mappings;

        /// <summary>
        ///     A Big Endian stream.
        /// </summary>
        private readonly FontFileStream _stream;

        /// <summary>
        ///     A dictionary of cached <see cref="FontTable" /> instances.
        ///     The index is the table name.
        /// </summary>
        private readonly IDictionary _tableCache = new Hashtable();

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="stream">Font data stream.</param>
        public FontFileReader( MemoryStream stream )
            : this( stream, string.Empty )
        {
        }

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="stream">Font data stream.</param>
        /// <param name="fontName">Name of a font in a TrueType collection.</param>
        public FontFileReader( MemoryStream stream, string fontName )
        {
            this._stream = new FontFileStream( stream );
            this._fontName = fontName;

            // Parse offset and directory tables
            ReadTableHeaders();

            // Caches commonly requested tables
            ReadRequiredTables();
        }

        /// <summary>
        ///     Gets or sets a dictionary containing glyph index to subset
        ///     index mappings.
        /// </summary>
        public IndexMappings IndexMappings
        {
            get
            {
                if ( _mappings == null )
                    _mappings = new IndexMappings();
                return _mappings;
            }
            set { _mappings = value; }
        }

        /// <summary>
        ///     Gets the underlying <see cref="FontFileStream" />.
        /// </summary>
        internal FontFileStream Stream
        {
            get { return _stream; }
        }

        /// <summary>
        ///     Gets the number tables.
        /// </summary>
        public int TableCount
        {
            get { return _header.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this font contains the
        ///     supplied table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <returns></returns>
        public bool ContainsTable( string tableName )
        {
            return _header.Contains( tableName );
        }

        /// <summary>
        ///     Gets a reference to the table structure identified by <i>tableName</i>
        /// </summary>
        /// <remarks>
        ///     Only the following tables are supported:
        ///     <see cref="TableNames.Head" /> - Font header,
        ///     <see cref="TableNames.Hhea" /> - Horizontal header,
        ///     <see cref="TableNames.Hmtx" /> - Horizontal metrics,
        ///     <see cref="TableNames.Maxp" /> - Maximum profile,
        ///     <see cref="TableNames.Loca" /> - Index to location,
        ///     <see cref="TableNames.Glyf" /> - Glyf data,
        ///     <see cref="TableNames.Cvt" /> - Control value,
        ///     <see cref="TableNames.Prep" /> - Control value program,
        ///     <see cref="TableNames.Fpgm" /> - Font program
        /// </remarks>
        /// <param name="tableName">A 4-character code identifying a table.</param>
        /// <exception cref="ArgumentException">
        ///     If <b>tableName</b> does not represent a table in this font.
        /// </exception>
        internal FontTable GetTable( string tableName )
        {
            if ( !ContainsTable( tableName ) )
                throw new ArgumentException( "Cannot locate table '" + tableName + "'", "tableName" );

            // Obtain from cache is present
            if ( _tableCache.Contains( tableName ) )
                return (FontTable)_tableCache[ tableName ];

            // Otherwise instantiate appropriate FontTable subclass and parse
            DirectoryEntry tableEntry = GetDictionaryEntry( tableName );
            FontTable table = tableEntry.MakeTable( this );
            if ( table != null )
            {
                OffsetStream( tableEntry );
                table.Read( this );
            }
            return table;
        }

        internal HeaderTable GetHeaderTable()
        {
            return (HeaderTable)GetTable( TableNames.Head );
        }

        internal MaximumProfileTable GetMaximumProfileTable()
        {
            return (MaximumProfileTable)GetTable( TableNames.Maxp );
        }

        internal HorizontalHeaderTable GetHorizontalHeaderTable()
        {
            return (HorizontalHeaderTable)GetTable( TableNames.Hhea );
        }

        internal HorizontalMetricsTable GetHorizontalMetricsTable()
        {
            return (HorizontalMetricsTable)GetTable( TableNames.Hmtx );
        }

        internal ControlValueTable GetControlValueTable()
        {
            return (ControlValueTable)GetTable( TableNames.Cvt );
        }

        internal ControlValueProgramTable GetControlValueProgramTable()
        {
            return (ControlValueProgramTable)GetTable( TableNames.Prep );
        }

        internal FontProgramTable GetFontProgramTable()
        {
            return (FontProgramTable)GetTable( TableNames.Fpgm );
        }

        internal GlyfDataTable GetGlyfDataTable()
        {
            return (GlyfDataTable)GetTable( TableNames.Glyf );
        }

        internal IndexToLocationTable GetIndexToLocationTable()
        {
            return (IndexToLocationTable)GetTable( TableNames.Loca );
        }

        internal Os2Table GetOs2Table()
        {
            return (Os2Table)GetTable( TableNames.Os2 );
        }

        internal PostTable GetPostTable()
        {
            return (PostTable)GetTable( TableNames.Post );
        }

        /// <summary>
        ///     Gets a <see cref="DirectoryEntry" /> object for the supplied table.
        /// </summary>
        /// <param name="tableName">A 4-character code identifying a table.</param>
        /// <returns>
        ///     A <see cref="DirectoryEntry" /> object or null if the table cannot
        ///     be located.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     If <b>tag</b> does not represent a table in this font.
        /// </exception>
        internal DirectoryEntry GetDictionaryEntry( string tableName )
        {
            if ( !ContainsTable( tableName ) )
                throw new ArgumentException( "Cannot locate table named " + tableName, "tableName" );

            return _header[ tableName ];
        }

        /// <summary>
        ///     Reads the Offset and Directory tables.  If the FontFileStream represents
        ///     a TrueType collection, this method will look for the aforementioned
        ///     tables belonging to <i>fontName</i>.
        /// </summary>
        /// <remarks>
        ///     This method can handle a TrueType collection.
        /// </remarks>
        protected void ReadTableHeaders()
        {
            // Check for possible TrueType collection
            string tag = Encoding.ASCII.GetString( _stream.ReadTag() );
            if ( tag == TableNames.Ttcf )
            {
                // Skip Version field - will be either 1.0 or 2.0
                _stream.Skip( PrimitiveSizes.ULong );

                // Number of fonts in TrueType collection
                var numFonts = (int)_stream.ReadULong();

                var foundFont = false;
                for ( var i = 0; i < numFonts && !foundFont; i++ )
                {
                    // Offset from beginning of file to a font's subtable
                    uint directoryOffset = _stream.ReadULong();

                    // Set a restore point since the code below will alter the stream position
                    _stream.SetRestorePoint();
                    _stream.Position = directoryOffset;

                    _header = new TrueTypeHeader();
                    _header.Read( _stream );

                    // To ascertain whether this font is the one we're looking for,
                    // we must read the 'name' table.
                    if ( !_header.Contains( TableNames.Name ) )
                        throw new Exception( "Unable to parse TrueType collection - missing 'head' table." );

                    // If font name is not supplied, select the first font in the colleciton;
                    // otherwise must have an exact match
                    var nameTable = (NameTable)GetTable( TableNames.Name );
                    if ( _fontName == string.Empty || nameTable.FullName == _fontName )
                        foundFont = true;

                    // Stream will now point to the next directory offset
                    _stream.Restore();
                }

                // We were unable to locate font in collection
                if ( !foundFont )
                    throw new Exception( "Unable to locate font '" + _fontName + "' in TrueType collection" );
            }
            else
            {
                _stream.Position = 0;

                // Read Offset and Directory tables
                _header = new TrueTypeHeader();
                _header.Read( _stream );
            }
        }

        /// <summary>
        ///     Caches the following tables: 'head', 'hhea', 'maxp', 'loca'
        /// </summary>
        protected void ReadRequiredTables()
        {
            _tableCache[ TableNames.Head ] = GetTable( TableNames.Head );
            _tableCache[ TableNames.Hhea ] = GetTable( TableNames.Hhea );
            _tableCache[ TableNames.Maxp ] = GetTable( TableNames.Maxp );
            _tableCache[ TableNames.Loca ] = GetTable( TableNames.Loca );
        }

        /// <summary>
        ///     Sets the stream position to the offset in the supplied directory
        ///     entry. Also ensures that the FontFileStream has enough bytes
        ///     available to read a font table.  Throws an exception if this
        ///     condition is not met.
        /// </summary>
        /// <param name="entry"></param>
        /// <exception cref="ArgumentException">
        ///     If the supplied stream does not contain enough data.
        /// </exception>
        private void OffsetStream( DirectoryEntry entry )
        {
            _stream.Position = entry.Offset;
            if ( _stream.Position + entry.Length > _stream.Length )
            {
                string msg = string.Format(
                    "Error reading table '{0}'.  Expected {1} bytes, current position {2}, stream length {3}",
                    entry.TableName, entry.Length, _stream.Position, _stream.Length );
                throw new ArgumentException( msg );
            }
        }
    }
}