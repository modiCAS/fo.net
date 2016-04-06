using System.Collections.Specialized;

namespace Fonet.Render.Pdf.Fonts
{
    /// <summary>
    ///     Represents a collection of font descriptor flags specifying
    ///     various characterisitics of a font.
    /// </summary>
    /// <remarks>
    ///     The following lists the bit positions and associated flags:
    ///     1  - FixedPitch
    ///     2  - Serif
    ///     3  - Symbolic
    ///     4  - Script
    ///     6  - Nonsymbolic
    ///     7  - Italic
    ///     17 - AllCap
    ///     18 - SmallCap
    ///     19 - ForceBold
    /// </remarks>
    internal sealed class FontDescriptorFlags
    {
        private BitVector32 _flags;

        /// <summary>
        ///     Default class constructor.
        /// </summary>
        public FontDescriptorFlags()
        {
            _flags = new BitVector32( 0 );
        }

        /// <summary>
        ///     Class constructor.  Initialises the flags BitVector with the
        ///     supplied integer.
        /// </summary>
        public FontDescriptorFlags( int flags )
        {
            this._flags = new BitVector32( flags );
        }

        /// <summary>
        ///     Gets the font descriptor flags as a 32-bit signed integer.
        /// </summary>
        public int Flags
        {
            get { return _flags.Data; }
        }

        public bool IsFixedPitch
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.FixedPitch ]; }
        }

        public bool IsSerif
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.Serif ]; }
        }

        public bool IsSymbolic
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.Symbolic ]; }
        }

        public bool IsScript
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.Script ]; }
        }

        public bool IsNonSymbolic
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.Nonsymbolic ]; }
        }

        public bool IsItalic
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.Italic ]; }
        }

        public bool IsAllCap
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.AllCap ]; }
        }

        public bool IsSmallCap
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.SmallCap ]; }
        }

        public bool IsForceBold
        {
            get { return _flags[ (int)FontDescriptorFlagsEnum.ForceBold ]; }
        }

        /// <summary>
        ///     Handy enumeration used to reference individual bit positions
        ///     in the BitVector32.
        /// </summary>
        internal enum FontDescriptorFlagsEnum
        {
            FixedPitch = 1,
            Serif = 2,
            Symbolic = 3,
            Script = 4,
            Nonsymbolic = 6,
            Italic = 7,
            AllCap = 17,
            SmallCap = 18,
            ForceBold = 19
        }
    }
}