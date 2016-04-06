namespace Fonet.Pdf
{
    /// <summary>
    ///     An International Color Code stream
    /// </summary>
    public class PdfIccStream : PdfStream
    {
        public PdfIccStream( PdfObjectId id, byte[] profileData )
            : base( id )
        {
            Data = profileData;
        }

        public PdfNumeric NumComponents
        {
            set { Dictionary[ PdfName.Names.N ] = value; }
        }

        public PdfString AlternativeColorSpace
        {
            set { Dictionary[ PdfName.Names.Alternate ] = value; }
        }
    }
}