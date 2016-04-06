using System.Text;

namespace Fonet.Pdf
{
    public class PdfVersion
    {
        public static readonly PdfVersion V14 = new PdfVersion( 1, 4 );
        public static readonly PdfVersion V13 = new PdfVersion( 1, 3 );
        public static readonly PdfVersion V12 = new PdfVersion( 1, 2 );
        public static readonly PdfVersion V11 = new PdfVersion( 1, 1 );
        public static readonly PdfVersion V10 = new PdfVersion( 1, 0 );

        private byte[] _header;

        private PdfVersion( byte major, byte minor )
        {
            Major = major;
            Minor = minor;
        }

        public byte[] Header
        {
            get
            {
                if ( _header == null )
                {
                    _header = Encoding.ASCII.GetBytes(
                        string.Format( "%PDF-{0}.{1}", Major, Minor ) );
                }
                return _header;
            }
        }

        public byte Major { get; private set; }

        public byte Minor { get; private set; }
    }
}