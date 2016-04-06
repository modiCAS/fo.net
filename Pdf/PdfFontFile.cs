using Fonet.Pdf.Filter;

namespace Fonet.Pdf
{
    public class PdfFontFile : PdfStream
    {
        public PdfFontFile( PdfObjectId id, byte[] fontData )
            : base( fontData, id )
        {
            AddFilter( new FlateFilter() );
            Dictionary[ PdfName.Names.Length1 ] = new PdfNumeric( fontData.Length );
        }
    }
}