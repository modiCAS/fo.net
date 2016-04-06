namespace Fonet.Pdf
{
    public class PdfXObject : PdfStream
    {
        private readonly byte[] objectData;

        public PdfXObject( byte[] objectData, PdfName name, PdfObjectId objectId )
            : base( objectId )
        {
            this.objectData = objectData;
            Name = name;
            dictionary[ PdfName.Names.Type ] = PdfName.Names.XObject;
        }

        public PdfName SubType
        {
            get { return (PdfName)dictionary[ PdfName.Names.Subtype ]; }
            set { dictionary[ PdfName.Names.Subtype ] = value; }
        }

        public PdfName Name { get; private set; }

        public PdfDictionary Dictionary
        {
            get { return dictionary; }
        }

        protected internal override void Write( PdfWriter writer )
        {
            data = objectData;
            base.Write( writer );
        }
    }
}