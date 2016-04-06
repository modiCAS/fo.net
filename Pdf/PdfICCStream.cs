namespace Fonet.Pdf
{
    /// <summary>
    ///     An International Color Code stream
    /// </summary>
    public class PdfICCStream : PdfStream
    {
        public PdfICCStream( PdfObjectId id, byte[] profileData )
            : base( id )
        {
            data = profileData;
        }

        public PdfNumeric NumComponents
        {
            set { dictionary[ PdfName.Names.N ] = value; }
        }

        public PdfString AlternativeColorSpace
        {
            set { dictionary[ PdfName.Names.Alternate ] = value; }
        }
    }
}