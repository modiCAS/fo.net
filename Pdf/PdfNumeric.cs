namespace Fonet.Pdf
{
    public sealed class PdfNumeric : PdfObject
    {
        private readonly decimal _val;

        public PdfNumeric( decimal val )
        {
            this._val = val;
        }

        public PdfNumeric( decimal val, PdfObjectId objectId )
            : base( objectId )
        {
            this._val = val;
        }

        protected internal override void Write( PdfWriter writer )
        {
            writer.Write( _val );
        }
    }
}