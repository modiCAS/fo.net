namespace Fonet.Pdf
{
    public class PdfXObject : PdfStream
    {
        private readonly byte[] _objectData;

        public PdfXObject( byte[] objectData, PdfName name, PdfObjectId objectId )
            : base( objectId )
        {
            _objectData = objectData;
            Name = name;
            Dictionary[ PdfName.Names.Type ] = PdfName.Names.XObject;
        }

        public PdfName SubType
        {
            get { return (PdfName)Dictionary[ PdfName.Names.Subtype ]; }
            set { Dictionary[ PdfName.Names.Subtype ] = value; }
        }

        public PdfName Name { get; private set; }

        protected internal override void Write( PdfWriter writer )
        {
            Data = _objectData;
            base.Write( writer );
        }
    }
}