using System.Collections;

namespace Fonet.Pdf.Gdi.Font
{
    internal class KerningPairs
    {
        /// <summary>
        ///     Key - Kerning pair identifier
        ///     Value - Kerning amount
        /// </summary>
        private readonly IDictionary _pairs;

        /// <summary>
        ///     Creates an instance of KerningPairs allocating space for
        ///     100 kerning pairs.
        /// </summary>
        public KerningPairs() : this( 100 )
        {
        }

        /// <summary>
        ///     Creates an instance of KerningPairs allocating space for
        ///     <i>numPairs</i> kerning pairs.
        /// </summary>
        /// <param name="numPairs"></param>
        public KerningPairs( int numPairs )
        {
            _pairs = new Hashtable( 100 );
        }

        /// <summary>
        ///     Gets the kerning amount for the supplied glyph index pair.
        /// </summary>
        public int this[ ushort left, ushort right ]
        {
            get
            {
                uint index = GetIndex( left, right );
                return _pairs.Contains( index ) ? (int)_pairs[ index ] : 0;
            }
        }

        /// <summary>
        ///     Gets the number of kernings pairs.
        /// </summary>
        public int Length
        {
            get { return _pairs.Count; }
        }

        /// <summary>
        ///     Returns true if a kerning value exists for the supplied
        ///     glyph index pair.
        /// </summary>
        /// <param name="left">Glyph index for left-hand glyph.</param>
        /// <param name="right">Glyph index for right-hand glyph.</param>
        /// <returns></returns>
        public bool HasKerning( ushort left, ushort right )
        {
            return _pairs.Contains( GetIndex( left, right ) );
        }

        /// <summary>
        ///     Creates a new kerning pair.
        /// </summary>
        /// <remarks>
        ///     This method will ignore duplicates.
        /// </remarks>
        /// <param name="left">The glyph index for the left-hand glyph in the kerning pair.</param>
        /// <param name="right">The glyph index for the right-hand glyph in the kerning pair. </param>
        /// <param name="value">The kerning value for the supplied pair.</param>
        internal void Add( ushort left, ushort right, int value )
        {
            if ( value != 0 )
            {
                uint index = GetIndex( left, right );
                if ( !_pairs.Contains( index ) )
                    _pairs[ index ] = value;
            }
        }

        /// <summary>
        ///     Returns a kerning pair identifier.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private uint GetIndex( ushort left, ushort right )
        {
            return (uint)( ( left << 16 ) + right );
        }
    }
}