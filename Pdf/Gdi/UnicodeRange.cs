using System;
using System.Text;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     Class that represents a unicode character range as returned
    ///     by the GetFontUnicodeRanges function.
    /// </summary>
    internal class UnicodeRange
    {
        private readonly GdiDeviceContent dc;

        /// <summary>
        ///     Array of glyph indices for each character represented by
        ///     this range begining at <see cref="Start" />.
        /// </summary>
        private ushort[] indices;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="start">Value representing start of unicode range.</param>
        /// <param name="end">Value representing end of unicode range.</param>
        public UnicodeRange( GdiDeviceContent dc, ushort start, ushort end )
        {
            if ( start > end )
                throw new ArgumentException( "start cannot be greater than end" );
            this.dc = dc;
            Start = start;
            End = end;
        }

        /// <summary>
        ///     Gets a value representing the start of the unicode range.
        /// </summary>
        public ushort Start { get; private set; }

        /// <summary>
        ///     Gets a value representing the end of the unicode range.
        /// </summary>
        public ushort End { get; private set; }

        /// <summary>
        ///     Returns the glyph index of <i>c</i>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public ushort MapCharacter( char c )
        {
            if ( indices == null )
                LoadGlyphIndices();
            return indices[ (ushort)( c - Start ) ];
        }

        /// <summary>
        ///     Populates the <i>indices</i> array with the glyph index of each
        ///     character represented by this rnage starting at <see cref="Start" />.
        /// </summary>
        private void LoadGlyphIndices()
        {
            string characters = BuildString();
            indices = new ushort[ characters.Length ];
            LibWrapper.GetGlyphIndices( dc.Handle, characters, characters.Length, indices, 0 );
        }

        private string BuildString()
        {
            var sb = new StringBuilder( End - Start );
            for ( ushort c = Start; c <= End; c++ )
                sb.Append( (char)c );
            return sb.ToString();
        }
    }
}