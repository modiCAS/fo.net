namespace Fonet.Pdf
{
    /// <summary>
    ///     A dictionary that contains information about a CIDFont program.
    /// </summary>
    /// <remarks>
    ///     A Type 0 CIDFont contains glyph descriptions based on Adobe's Type
    ///     1 font format, whereas those in a Type 2 CIDFont are based on the
    ///     TrueType font format.
    /// </remarks>
    public class PdfCidFont : PdfDictionary
    {
        public PdfCidFont(
            PdfObjectId objectId, PdfFontSubTypeEnum subType, string baseFont )
            : base( objectId )
        {
            this[ PdfName.Names.Type ] = PdfName.Names.Font;
            this[ PdfName.Names.Subtype ] = new PdfName( subType.ToString() );
            this[ PdfName.Names.BaseFont ] = new PdfName( baseFont );
            this[ PdfName.Names.Dw ] = new PdfNumeric( 1000 );
            this[ PdfName.Names.CidtoGidMap ] = PdfName.Names.Identity;
        }

        public PdfCidSystemInfo SystemInfo
        {
            set { this[ PdfName.Names.CidSystemInfo ] = value; }
        }

        public PdfFontDescriptor Descriptor
        {
            set { this[ PdfName.Names.FontDescriptor ] = value.GetReference(); }
        }

        public PdfNumeric DefaultWidth
        {
            set { this[ PdfName.Names.Dw ] = value; }
        }

        public PdfWArray Widths
        {
            set { this[ PdfName.Names.W ] = value; }
        }
    }
}