using System.Collections;

namespace Fonet.Pdf
{
    /// <summary>
    ///     A collection of <see cref="BfEntry" /> instances.
    /// </summary>
    internal class BfEntryList : IEnumerable
    {
        private readonly ArrayList _entries = new ArrayList();

        /// <summary>
        ///     Gets the <see cref="BfEntry" /> at <i>index</i>.
        /// </summary>
        public BfEntry this[ int index ]
        {
            get { return (BfEntry)_entries[ index ]; }
        }

        /// <summary>
        ///     Gets the number of <see cref="BfEntry" /> objects contained by this
        ///     <see cref="BfEntryList" />
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        ///     Returns the number of <see cref="BfEntry" /> instances that
        ///     represent bfrange's
        /// </summary>
        /// <returns></returns>
        public int NumRanges
        {
            get
            {
                var count = 0;
                foreach ( BfEntry entry in _entries )
                {
                    if ( entry.IsRange )
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// </summary>
        public BfEntry[] Ranges
        {
            get
            {
                var ranges = new ArrayList( NumRanges );
                foreach ( BfEntry entry in this )
                {
                    if ( entry.IsRange )
                        ranges.Add( entry );
                }
                return (BfEntry[])ranges.ToArray( typeof( BfEntry ) );
            }
        }

        /// <summary>
        ///     Returns the number of <see cref="BfEntry" /> instances that
        ///     represent bfchar's
        /// </summary>
        /// <returns></returns>
        public int NumChars
        {
            get { return _entries.Count - NumRanges; }
        }

        /// <summary>
        /// </summary>
        public BfEntry[] Chars
        {
            get
            {
                var chars = new ArrayList( NumChars );
                foreach ( BfEntry entry in this )
                {
                    if ( entry.IsChar )
                        chars.Add( entry );
                }
                return (BfEntry[])chars.ToArray( typeof( BfEntry ) );
            }
        }

        /// <summary>
        ///     Returns an ArrayList enumerator that references a read-only version
        ///     of the BfEntry list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ArrayList.ReadOnly( _entries ).GetEnumerator();
        }

        /// <summary>
        ///     Adds the supplied <see cref="BfEntry" /> to the end of the collection.
        /// </summary>
        /// <param name="entry"></param>
        public void Add( BfEntry entry )
        {
            _entries.Add( entry );
        }
    }

    /// <summary>
    ///     A <see cref="BfEntry" /> class can represent either a bfrange
    ///     or bfchar.
    /// </summary>
    internal class BfEntry
    {
        /// <summary>
        ///     Class cosntructor.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="unicodeValue"></param>
        public BfEntry( ushort startIndex, ushort unicodeValue )
        {
            StartGlyphIndex = startIndex;
            EndGlyphIndex = startIndex;
            UnicodeValue = unicodeValue;
        }

        public ushort StartGlyphIndex { get; private set; }

        public ushort EndGlyphIndex { get; private set; }

        public ushort UnicodeValue { get; private set; }

        /// <summary>
        ///     Returns <b>true</b> if this BfEntry represents a glyph range, i.e.
        ///     the start index is not equal to the end index.
        /// </summary>
        public bool IsRange
        {
            get { return StartGlyphIndex != EndGlyphIndex; }
        }

        /// <summary>
        ///     Returns <b>true</b> if this BfEntry represents a bfchar entry, i.e.
        ///     the start index is equal to the end index.
        /// </summary>
        public bool IsChar
        {
            get { return !IsRange; }
        }

        /// <summary>
        ///     Increments the end index by one.
        /// </summary>
        /// <remarks>
        ///     Incrementing the end index turns this BfEntry into a bfrange.
        /// </remarks>
        public void IncrementEndIndex()
        {
            EndGlyphIndex++;
        }
    }
}