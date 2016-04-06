using System.IO;
using System.IO.Compression;

namespace Fonet.Pdf.Filter
{
    public class FlateFilter : IFilter
    {
        public PdfObject Name
        {
            get { return PdfName.Names.FlateDecode; }
        }

        public PdfObject DecodeParms
        {
            get { return PdfNull.Null; }
        }

        public bool HasDecodeParams
        {
            get { return false; }
        }

        public byte[] Encode( byte[] data )
        {
            var ms = new MemoryStream( data.Length );
            ms.WriteByte( 0x78 ); // ZLib Header for compression level 3.
            ms.WriteByte( 0x5e );
            var ds = new DeflateStream( ms, CompressionMode.Compress );
            ds.Write( data, 0, data.Length );
            ds.Close();
            return ms.ToArray();
        }
    }
}