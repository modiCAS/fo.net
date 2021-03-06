namespace Fonet.Pdf.Filter
{
    public class DctFilter : IFilter
    {
        public PdfObject Name
        {
            get { return PdfName.Names.DctDecode; }
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