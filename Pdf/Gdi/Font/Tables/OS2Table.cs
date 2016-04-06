using System;

namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the OS/2 ('OS/2') table
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         For detailed information on the OS/2 table, visit the following link:
    ///         http://www.microsoft.com/typography/otspec/os2.htm
    ///     </p>
    ///     <p>
    ///         For more details on the Panose classification metrics, visit the following URL:
    ///         http://www.panose.com/hardware/pan2.asp
    ///     </p>
    /// </remarks>
    internal class Os2Table : FontTable
    {
        private const int OldStyleSerifs = 1;
        private const int TransitionalSerifs = 2;
        private const int ModernSerifs = 3;
        private const int ClarendonSerifs = 4;
        private const int SlabSerifs = 5;
        private const int FreeformSerifs = 7;
        private const int SansSerif = 8;
        private const int Scripts = 10;
        private const int Symbolic = 12;
        private short _avgCharWidth;
        private byte _classID;
        private uint _codePageRange1;
        private uint _codePageRange2;
        private ushort _fsSelection;
        private ushort _fsType;
        private readonly byte[] _panose = new byte[ 10 ];
        private short _sCapHeight;
        private short _strikeoutPosition;
        private short _strikeoutSize;
        private byte _subclassID;
        private short _subscriptXOffset;
        private short _subscriptXSize;
        private short _subscriptYOffset;
        private short _subscriptYSize;
        private short _superscriptXOffset;
        private short _superscriptXSize;
        private short _superscriptYOffset;
        private short _superscriptYSize;
        private short _sxHeight;
        private short _typoAscender;
        private short _typoDescender;
        private short _typoLineGap;
        private uint _unicodeRange1;
        private uint _unicodeRange2;
        private uint _unicodeRange3;
        private uint _unicodeRange4;
        private ushort _usBreakChar;
        private ushort _usDefaultChar;
        private ushort _usMaxContext;
        private ushort _usWeightClass;
        private ushort _usWidthClass;
        private ushort _usWinAscent;
        private ushort _usWinDescent;
        private readonly sbyte[] _vendorID = new sbyte[ 4 ];

        private ushort _version;

        public Os2Table( DirectoryEntry entry ) : base( TableNames.Os2, entry )
        {
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains
        ///     italic characters.
        /// </summary>
        public bool IsItalic
        {
            get { return ( _fsSelection & 0x01 ) == 0x01; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters are
        ///     in the standard weight/style.
        /// </summary>
        public bool IsRegular
        {
            get { return ( _fsSelection & 0x40 ) == 0x40; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters possess
        ///     a weight greater than or equal to 700.
        /// </summary>
        public bool IsBold
        {
            get { return ( _fsSelection & 0x20 ) == 0x20 || _usWeightClass >= 700; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains
        ///     characters that all have the same width.
        /// </summary>
        public bool IsMonospaced
        {
            get { return _panose[ 3 ] == 9; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font contains
        ///     special characters such as dingbats, icons, etc.
        /// </summary>
        public bool IsSymbolic
        {
            get { return _classID == Symbolic; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters
        ///     do possess serifs
        /// </summary>
        public bool IsSerif
        {
            get
            {
                return _classID == OldStyleSerifs ||
                    _classID == TransitionalSerifs ||
                    _classID == ModernSerifs ||
                    _classID == ClarendonSerifs ||
                    _classID == SlabSerifs ||
                    _classID == FreeformSerifs;
            }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters
        ///     are designed to simulate hand writing.
        /// </summary>
        public bool IsScript
        {
            get { return _classID == Scripts; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether characters
        ///     do not possess serifs
        /// </summary>
        public bool IsSansSerif
        {
            get { return _classID == SansSerif; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font may be
        ///     legally embedded.
        /// </summary>
        public bool IsEmbeddable
        {
            get { return InstallableEmbedding || EditableEmbedding || PreviewAndPrintEmbedding; }
        }

        public bool InstallableEmbedding
        {
            get { return _fsType == 0; }
        }

        public bool RestricedLicenseEmbedding
        {
            get { return ( _fsType & 0x0002 ) == 0x0002; }
        }

        public bool EditableEmbedding
        {
            get { return ( _fsType & 0x0008 ) == 0x0008; }
        }

        public bool PreviewAndPrintEmbedding
        {
            get { return ( _fsType & 0x0004 ) == 0x0004; }
        }

        /// <summary>
        ///     Gets a boolean value that indicates whether this font may be
        ///     subsetted.
        /// </summary>
        public bool IsSubsettable
        {
            get { return ( _fsType & 0x0100 ) != 0x0100; }
        }

        public int CapHeight
        {
            get { return _sCapHeight; }
        }

        public int XHeight
        {
            get { return _sxHeight; }
        }

        public ushort FirstChar { get; private set; }

        public ushort LastChar { get; private set; }

        /// <summary>
        ///     Reads the contents of the "os/2" table from the supplied stream
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;
            _version = stream.ReadUShort();
            _avgCharWidth = stream.ReadShort();
            _usWeightClass = stream.ReadUShort();
            _usWidthClass = stream.ReadUShort();
            // According to the OpenType spec, bit 0 must be zero.
            _fsType = (ushort)( stream.ReadUShort() & ~1 );
            _subscriptXSize = stream.ReadShort();
            _subscriptYSize = stream.ReadShort();
            _subscriptXOffset = stream.ReadShort();
            _subscriptYOffset = stream.ReadShort();
            _superscriptXSize = stream.ReadShort();
            _superscriptYSize = stream.ReadShort();
            _superscriptXOffset = stream.ReadShort();
            _superscriptYOffset = stream.ReadShort();
            _strikeoutSize = stream.ReadShort();
            _strikeoutPosition = stream.ReadShort();
            short familyClass = stream.ReadShort();
            _classID = (byte)( familyClass >> 8 );
            _subclassID = (byte)( familyClass & 255 );
            stream.Read( _panose, 0, _panose.Length );
            _unicodeRange1 = stream.ReadULong();
            _unicodeRange2 = stream.ReadULong();
            _unicodeRange3 = stream.ReadULong();
            _unicodeRange4 = stream.ReadULong();
            _vendorID[ 0 ] = stream.ReadChar();
            _vendorID[ 1 ] = stream.ReadChar();
            _vendorID[ 2 ] = stream.ReadChar();
            _vendorID[ 3 ] = stream.ReadChar();
            _fsSelection = stream.ReadUShort();
            FirstChar = stream.ReadUShort();
            LastChar = stream.ReadUShort();
            _typoAscender = stream.ReadShort();
            _typoDescender = stream.ReadShort();
            _typoLineGap = stream.ReadShort();
            _usWinAscent = stream.ReadUShort();
            _usWinDescent = stream.ReadUShort();
            _codePageRange1 = stream.ReadULong();
            _codePageRange2 = stream.ReadULong();
            _sxHeight = stream.ReadShort();
            _sCapHeight = stream.ReadShort();
            _usDefaultChar = stream.ReadUShort();
            _usBreakChar = stream.ReadUShort();
            _usMaxContext = stream.ReadUShort();
        }

        protected internal override void Write( FontFileWriter writer )
        {
            throw new NotImplementedException( "Write is not implemented." );
        }
    }
}