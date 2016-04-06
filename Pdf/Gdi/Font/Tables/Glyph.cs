using System.Collections;

namespace Fonet.Pdf.Gdi.Font
{
    /// <summary>
    ///     Represents either a simple or composite glyph description from
    ///     the 'glyf' table.
    /// </summary>
    /// <remarks>
    ///     This class is nothing more than a wrapper around
    ///     a byte array.
    /// </remarks>
    internal class Glyph
    {
        /// <summary>
        ///     List of composite glyph indices.
        /// </summary>
        private readonly IList _children;

        /// <summary>
        ///     Contains glyph description as raw data.
        /// </summary>
        private byte[] _glyphData;

        /// <summary>
        ///     The index of this glyph as obtained from the 'loca' table.
        /// </summary>
        private readonly int _glyphIndex;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public Glyph( int glyphIndex )
        {
            this._glyphIndex = glyphIndex;
            _children = new ArrayList();
        }

        /// <summary>
        ///     Gets or sets the index of this glyph.
        /// </summary>
        public int Index
        {
            get { return _glyphIndex; }
        }

        /// <summary>
        ///     Gets the length of the glyph data buffer.
        /// </summary>
        public uint Length
        {
            get { return _glyphData != null ? (uint)_glyphData.Length : 0; }
        }

        /// <summary>
        ///     Gets a ilst of child glyph indices.
        /// </summary>
        public IList Children
        {
            get { return _children; }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this glyph represents
        ///     a composite glyph.
        /// </summary>
        public bool IsComposite
        {
            get { return _children.Count != 0; }
        }

        /// <summary>
        ///     Sets the glyph data (duh!).
        /// </summary>
        /// <param name="glyphData"></param>
        public void SetGlyphData( byte[] glyphData )
        {
            this._glyphData = glyphData;
        }

        /// <summary>
        ///     Add the supplied glyph index to list of children.
        /// </summary>
        /// <param name="glyphIndex"></param>
        public void AddChild( int glyphIndex )
        {
            _children.Add( glyphIndex );
        }

        /// <summary>
        ///     Writes a glyph description to the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Write( FontFileStream stream )
        {
            if ( _glyphData != null && _glyphData.Length > 0 )
                stream.Write( _glyphData, 0, _glyphData.Length );
        }
    }
}