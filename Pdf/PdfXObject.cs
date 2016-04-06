namespace Fonet.Pdf
{
    public class PdfXObject : PdfStream
    {
        private readonly byte[] _objectData;

        public PdfXObject( byte[] objectData, PdfName name, PdfObjectId objectId )
            : base( objectId )
        {
            this._objectData = objectData;
            Name = name;
            base.Dictionary[ PdfName.Names.Type ] = PdfName.Names.XObject;
        }

        public PdfName SubType
        {
            get { return (PdfName)base.Dictionary[ PdfName.Names.Subtype ]; }
            set { base.Dictionary[ PdfName.Names.Subtype ] = value; }
        }

        public PdfName Name { get; private set; }

        protected internal override void Write( PdfWriter writer )
        {
            Data = _objectData;
            base.Write( writer );
        }
    }
}