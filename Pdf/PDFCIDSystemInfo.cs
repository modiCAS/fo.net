namespace Fonet.Pdf
{
    /// <summary>
    ///     A dictionary containing entries that define the character collection
    ///     of the CIDFont.
    /// </summary>
    public class PdfCidSystemInfo : PdfDictionary
    {
        public const string DefaultRegistry = "Adobe";
        public const string DefaultOrdering = "Identity";
        public const int DefaultSupplement = 0;

        public PdfCidSystemInfo()
        {
            this[ PdfName.Names.Registry ] = new PdfString( DefaultRegistry );
            this[ PdfName.Names.Ordering ] = new PdfString( DefaultOrdering );
            this[ PdfName.Names.Supplement ] = new PdfNumeric( DefaultSupplement );
        }

        public PdfCidSystemInfo(
            string registry, string ordering, int supplement )
        {
            this[ PdfName.Names.Registry ] = new PdfString( registry );
            this[ PdfName.Names.Ordering ] = new PdfString( ordering );
            this[ PdfName.Names.Supplement ] = new PdfNumeric( supplement );
        }
    }
}