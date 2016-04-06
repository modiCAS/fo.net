namespace Fonet.Pdf.Filter
{
    public class LzwFilter : IFilter
    {
        public LzwFilter()
        {
            throw new UnsupportedFilterException( "LZWDecode" );
        }

        public PdfObject Name
        {
            get { return PdfName.Names.LzwDecode; }
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
            return data;
        }
    }
}