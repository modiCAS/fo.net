using System;
using System.IO;
using System.Text;
using Fonet.DataTypes;

namespace Fonet.Image
{
    /// <summary>
    ///     Parses the contents of a JPEG image header to infer the colour
    ///     space and bits per pixel.
    /// </summary>
    internal sealed class JpegParser
    {
        public const int MSof0 = 0xC0; /* Start Of Frame N */
        public const int MSof1 = 0xC1; /* N indicates which compression process */
        public const int MSof2 = 0xC2; /* Only SOF0-SOF2 are now in common use */
        public const int MSof3 = 0xC3;
        public const int MSof5 = 0xC5; /* NB: codes C4 and CC are NOT SOF markers */
        public const int MSof6 = 0xC6;
        public const int MSof7 = 0xC7;
        public const int MSof9 = 0xC9;
        public const int MSof10 = 0xCA;
        public const int MSof11 = 0xCB;
        public const int MSof13 = 0xCD;
        public const int MSof14 = 0xCE;
        public const int MSof15 = 0xCF;
        public const int MSoi = 0xD8; /* Start Of Image (beginning of datastream) */
        public const int MEoi = 0xD9; /* End Of Image (end of datastream) */
        public const int MSos = 0xDA; /* Start Of Scan (begins compressed data) */
        public const int MApp0 = 0xE0; /* Application-specific marker, type N */
        public const int MApp1 = 0xE1;
        public const int MApp2 = 0xE2;
        public const int MApp3 = 0xE3;
        public const int MApp4 = 0xE4;
        public const int MApp5 = 0xE5;
        public const int MApp12 = 0xEC; /* (we don't bother to list all 16 APPn's) */
        public const int MCom = 0xFE; /* COMment */

        public const string IccProfile = "ICC_PROFILE\0";

        /// <summary>
        ///     Contains number of bitplanes, color space and optional ICC Profile
        /// </summary>
        private readonly JpegInfo _headerInfo;

        /// <summary>
        ///     Raw ICC Profile
        /// </summary>
        private MemoryStream _iccProfileData;

        /// <summary>
        ///     JPEG image data
        /// </summary>
        private readonly MemoryStream _ms;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="data"></param>
        public JpegParser( byte[] data )
        {
            _ms = new MemoryStream( data );
            _headerInfo = new JpegInfo();
        }

        public JpegInfo Parse()
        {
            // File must begin with SOI marker
            if ( ReadFirstMarker() != MSoi )
                throw new InvalidOperationException( "Expected SOI marker first" );

            while ( _ms.Position < _ms.Length )
            {
                int marker = ReadNextMarker();
                switch ( marker )
                {
                case MSof0: // Baseline
                case MSof1: // Extended sequential, Huffman
                case MSof2: // Progressive, Huffman
                case MSof3: // Lossless, Huffman
                case MSof5: // Differential sequential, Huffman
                case MSof6: // Differential progressive, Huffman
                case MSof7: // Differential lossless, Huffman
                case MSof9: // Extended sequential, Huffman
                case MSof10: // Progressive, arithmetic
                case MSof11: // Lossless, arithmetic
                case MSof13: // Differential sequential, arithmetic
                case MSof14: // Differential progressive, arithmetic
                case MSof15: // Differential lossless, arithmetic
                    ReadHeader();
                    break;
                case MApp2: // ICC Profile
                    ReadIccProfile();
                    break;

                default:
                    SkipVariable();
                    break;
                }
            }

            if ( _iccProfileData != null )
                _headerInfo.SetIccProfile( _iccProfileData.ToArray() );

            return _headerInfo;
        }

        private void ReadIccProfile()
        {
            if ( _iccProfileData == null )
                _iccProfileData = new MemoryStream();

            // Length of entire block in bytes
            int length = ReadInt();

            // Should be the string constant "ICC_PROFILE"
            string iccProfile = ReadString( 12 );
            if ( !iccProfile.Equals( IccProfile ) )
                throw new Exception( "Missing ICC_PROFILE identifier in APP2 block" );

            ReadByte(); // Sequence number of block
            ReadByte(); // Total number of markers

            // Accumulate profile data in temporary memory stream
            var profileData = new byte[ length - 16 ];
            _ms.Read( profileData, 0, profileData.Length );

            _iccProfileData.Write( profileData, 0, profileData.Length );
        }

        /// <summary>
        /// </summary>
        private void ReadHeader()
        {
            ReadInt(); // Length of block

            _headerInfo.SetBitsPerSample( ReadByte() );
            _headerInfo.SetHeight( ReadInt() );
            _headerInfo.SetWidth( ReadInt() );
            _headerInfo.SetNumColourComponents( ReadByte() );
        }

        /// <summary>
        ///     Reads a 16-bit integer from the underlying stream
        /// </summary>
        /// <returns></returns>
        private int ReadInt()
        {
            return ( ReadByte() << 8 ) + ReadByte();
        }

        /// <summary>
        ///     Reads a 32-bit integer from the underlying stream
        /// </summary>
        /// <returns></returns>
        private byte ReadByte()
        {
            return (byte)_ms.ReadByte();
        }

        /// <summary>
        ///     Reads the specified number of bytes from theunderlying stream
        ///     and converts them to a string using the ASCII encoding.
        /// </summary>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        private string ReadString( int numBytes )
        {
            var name = new byte[ numBytes ];
            _ms.Read( name, 0, name.Length );

            return Encoding.ASCII.GetString( name );
        }

        /// <summary>
        ///     Reads the initial marker which should be SOI.
        /// </summary>
        /// <remarks>
        ///     After invoking this method the stream will point to the location
        ///     immediately after the fiorst marker.
        /// </remarks>
        /// <returns></returns>
        private int ReadFirstMarker()
        {
            int b1 = _ms.ReadByte();
            int b2 = _ms.ReadByte();
            if ( b1 != 0xFF || b2 != MSoi )
                throw new InvalidOperationException( "Not a JPEG file" );

            return b2;
        }

        /// <summary>
        ///     Reads the next JPEG marker and returns its marker code.
        /// </summary>
        /// <returns></returns>
        private int ReadNextMarker()
        {
            // Skip stream contents until we reach a FF tag
            int b = _ms.ReadByte();
            while ( b != 0xFF )
                b = _ms.ReadByte();

            // Skip any FF padding bytes
            do
            {
                b = _ms.ReadByte();
            }
            while ( b == 0xFF );

            return b;
        }

        /// <summary>
        ///     Skips over the parameters for any marker we don't want to process.
        /// </summary>
        private void SkipVariable()
        {
            int length = ReadInt();

            // Length includes itself, therefore it must be at least 2
            if ( length < 2 )
                throw new InvalidOperationException( "Invalid JPEG marker length" );

            // Skip all parameters
            _ms.Seek( length - 2, SeekOrigin.Current );
        }
    }

    internal class JpegInfo
    {
        private int _colourSpace = ColorSpace.DeviceUnknown;

        public byte[] IccProfileData { get; private set; }

        public bool HasIccProfile
        {
            get { return IccProfileData != null; }
        }

        public int ColourSpace
        {
            get { return _colourSpace; }
        }

        public int BitsPerSample { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal void SetNumColourComponents( int colourComponents )
        {
            // Translate number of colur components into a ColourSpace constant
            switch ( colourComponents )
            {
            case 1:
                _colourSpace = ColorSpace.DeviceGray;
                break;
            case 3:
                _colourSpace = ColorSpace.DeviceRgb;
                break;
            case 4:
                _colourSpace = ColorSpace.DeviceCmyk;
                break;
            default:
                _colourSpace = ColorSpace.DeviceUnknown;
                break;
            }
        }

        internal void SetBitsPerSample( int bitsPerSample )
        {
            BitsPerSample = bitsPerSample;
        }

        internal void SetWidth( int width )
        {
            Width = width;
        }

        internal void SetHeight( int height )
        {
            Height = height;
        }

        internal void SetIccProfile( byte[] profileData )
        {
            IccProfileData = profileData;
        }
    }
}