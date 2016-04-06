using System.Collections;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Utility class that stores a list of glyph indices and their
    ///     asociated subset indices.
    /// </summary>
    public class IndexMappings
    {
        /// <summary>
        ///     Maps a glyph index to a subset index.
        /// </summary>
        private readonly SortedList _glyphToSubset;

        /// <summary>
        ///     Maps a subset index to glyph index.
        /// </summary>
        private readonly SortedList _subsetToGlyph;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public IndexMappings()
        {
            _glyphToSubset = new SortedList();
            _subsetToGlyph = new SortedList();
        }

        /// <summary>
        ///     Gets the number of glyph to subset index mappings.
        /// </summary>
        public int Count
        {
            get { return _glyphToSubset.Count; }
        }

        /// <summary>
        ///     Gets a list of glyph indices sorted in ascending order.
        /// </summary>
        public IList GlyphIndices
        {
            get { return new ArrayList( _glyphToSubset.Keys ); }
        }

        /// <summary>
        ///     Gets a list of subset indices sorted in ascending order.
        /// </summary>
        public IList SubsetIndices
        {
            get { return new ArrayList( _subsetToGlyph.Keys ); }
        }

        /// <summary>
        ///     Determines whether a mapping exists for the supplied glyph index.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns></returns>
        public bool HasMapping( int glyphIndex )
        {
            return _glyphToSubset.Contains( glyphIndex );
        }

        /// <summary>
        ///     Returns the subset index for <i>glyphIndex</i>.  If a subset
        ///     index does not exist for <i>glyphIndex</i> one is generated.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns>A subset index.</returns>
        public int Map( int glyphIndex )
        {
            var subsetIndex = 0;
            if ( _glyphToSubset.Contains( glyphIndex ) )
                subsetIndex = (int)_glyphToSubset[ glyphIndex ];
            else
            {
                subsetIndex = _glyphToSubset.Count;
                _glyphToSubset.Add( glyphIndex, subsetIndex );
                _subsetToGlyph.Add( subsetIndex, glyphIndex );
            }
            return subsetIndex;
        }

        /// <summary>
        ///     Adds the list of supplied glyph indices to the index mappings using
        ///     the next available subset index for each glyph index.
        /// </summary>
        /// <param name="glyphIndices"></param>
        public void Add( params int[] glyphIndices )
        {
            foreach ( int index in glyphIndices )
                Map( index );
        }

        /// <summary>
        ///     Gets the subset index of <i>glyphIndex</i>.
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns>
        ///     A glyph index or <b>-1</b> if a glyph to subset mapping does not exist.
        /// </returns>
        public int GetSubsetIndex( int glyphIndex )
        {
            if ( _glyphToSubset.Contains( glyphIndex ) )
                return (int)_glyphToSubset[ glyphIndex ];
            return -1;
        }

        /// <summary>
        ///     Gets the glyph index of <i>subsetIndex</i>.
        /// </summary>
        /// <param name="subsetIndex"></param>
        /// <returns>
        ///     A subset index or <b>-1</b> if a subset to glyph mapping does not exist.
        /// </returns>
        public int GetGlyphIndex( int subsetIndex )
        {
            if ( _subsetToGlyph.Contains( subsetIndex ) )
                return (int)_subsetToGlyph[ subsetIndex ];
            return -1;
        }
    }
}