using System;
using System.Collections;

namespace Fonet.Pdf
{
    /// <summary>
    ///     Class that defines a mapping between character codes (CIDs)
    ///     to a character selector (Identity-H encoding)
    /// </summary>
    public class PdfCMap : PdfContentStream
    {
        public const string DefaultName = "Adobe-Identity-UCS";

        private readonly SortedList _ranges;

        private PdfCidSystemInfo _systemInfo;

        public PdfCMap( PdfObjectId id )
            : base( id )
        {
            _ranges = new SortedList();
        }

        public PdfCidSystemInfo SystemInfo
        {
            set { _systemInfo = value; }
        }

        /// <summary>
        ///     Adds the supplied glyph -> unicode pairs.
        /// </summary>
        /// <remarks>
        ///     Both the key and value must be a ushort.
        /// </remarks>
        /// <param name="map"></param>
        public void AddBfRanges( IDictionary map )
        {
            foreach ( DictionaryEntry entry in map )
            {
                AddBfRange(
                    Convert.ToUInt16( entry.Key ),
                    Convert.ToUInt16( entry.Value ) );
            }
        }

        /// <summary>
        ///     Adds the supplied glyph index to unicode value mapping.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <param name="unicodeValue"></param>
        public void AddBfRange( ushort glyphIndex, ushort unicodeValue )
        {
            _ranges.Add( glyphIndex, unicodeValue );
        }

        /// <summary>
        ///     Overriden to create CMap content stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write( PdfWriter writer )
        {
            WriteLine( "/CIDInit /ProcSet findresource begin" );
            WriteLine( "12 dict begin" );
            WriteLine( "begincmap" );
            WriteLine( "/CIDSystemInfo" );
            WriteLine( _systemInfo );
            WriteLine( "def" );
            WriteLine( string.Format( "/CMapName /{0} def", DefaultName ) );
            WriteLine( "/CMapType 2 def" );

            // No bfranges represents an error - we should really through an exception
            if ( _ranges.Count > 0 )
            {
                // Groups CMap entries into bfranges
                BfEntryList groups = GroupCMapEntries();

                // Write out the codespace ranges
                WriteCodespaceRange( groups );

                // Write out GID to Unicode mappings
                WriteBfChars( groups );
                WriteBfRanges( groups );
            }

            WriteLine( "endcmap" );
            WriteLine( "CMapName currentdict /CMap defineresource pop" );
            WriteLine( "end" );
            Write( "end" );

            base.Write( writer );
        }

        private void WriteCodespaceRange( BfEntryList entries )
        {
            BfEntry first = entries[ 0 ];
            BfEntry last = entries[ entries.Count - 1 ];

            WriteLine( "1 begincodespacerange" );
            WriteLine( string.Format( "<{0:X4}> <{1:X4}>",
                first.StartGlyphIndex, last.EndGlyphIndex ) );
            WriteLine( "endcodespacerange" );
        }

        /// <summary>
        ///     Writes the bfchar entries to the content stream in groups of 100.
        /// </summary>
        /// <param name="entries"></param>
        private void WriteBfChars( BfEntryList entries )
        {
            // bfchar entries must be grouped in blocks of 100
            BfEntry[] charEntries = entries.Chars;
            int numBlocks = charEntries.Length / 100 + ( charEntries.Length % 100 > 0 ? 1 : 0 );

            for ( var i = 0; i < numBlocks; i++ )
            {
                var blockSize = 0;
                if ( i + 1 == numBlocks )
                    blockSize = charEntries.Length - i * 100;
                else
                    blockSize = 100;

                WriteLine( string.Format( "{0} beginbfchar", blockSize ) );

                for ( var j = 0; j < blockSize; j++ )
                {
                    BfEntry entry = charEntries[ i * 100 + j ];
                    WriteLine( string.Format( "<{0:X4}> <{1:X4}>",
                        entry.StartGlyphIndex,
                        entry.UnicodeValue ) );
                }
                WriteLine( "endbfchar" );
            }
        }

        /// <summary>
        ///     Writes the bfrange entries to the content stream in groups of 100.
        /// </summary>
        /// <param name="entries"></param>
        private void WriteBfRanges( BfEntryList entries )
        {
            // bfrange entries must be grouped in blocks of 100
            BfEntry[] rangeEntries = entries.Ranges;
            int numBlocks = rangeEntries.Length / 100 + ( rangeEntries.Length % 100 > 0 ? 1 : 0 );

            for ( var i = 0; i < numBlocks; i++ )
            {
                var blockSize = 0;
                if ( i + 1 == numBlocks )
                    blockSize = rangeEntries.Length - i * 100;
                else
                    blockSize = 100;

                WriteLine( string.Format( "{0} beginbfrange", blockSize ) );

                for ( var j = 0; j < blockSize; j++ )
                {
                    BfEntry entry = rangeEntries[ i * 100 + j ];
                    WriteLine( string.Format( "<{0:X4}> <{1:X4}> <{2:X4}>",
                        entry.StartGlyphIndex,
                        entry.EndGlyphIndex,
                        entry.UnicodeValue ) );
                }
                WriteLine( "endbfrange" );
            }
        }

        private BfEntryList GroupCMapEntries()
        {
            // List of grouped bfranges
            var groups = new BfEntryList();

            var prevGlyphIndex = (ushort)_ranges.GetKey( 0 );
            var prevUnicodeValue = (ushort)_ranges[ prevGlyphIndex ];
            var range = new BfEntry( prevGlyphIndex, prevUnicodeValue );
            groups.Add( range );

            for ( var i = 1; i < _ranges.Count; i++ )
            {
                var glyphIndex = (ushort)_ranges.GetKey( i );
                var unicodeValue = (ushort)_ranges[ glyphIndex ];

                if ( unicodeValue == prevUnicodeValue + 1 &&
                    glyphIndex == prevGlyphIndex + 1 )
                {
                    // Contingous block - use existing range
                    range.IncrementEndIndex();
                }
                else
                {
                    // Non-contiguous block - start new range
                    range = new BfEntry( glyphIndex, unicodeValue );
                    groups.Add( range );
                }

                prevGlyphIndex = glyphIndex;
                prevUnicodeValue = unicodeValue;
            }

            return groups;
        }
    }
}