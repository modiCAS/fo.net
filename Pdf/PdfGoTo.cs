namespace Fonet.Pdf
{
    public sealed class PdfGoTo : PdfDictionary, IPdfAction
    {
        private PdfObjectReference _pageReference;

        private decimal _xPosition;

        private decimal _yPosition;

        public PdfGoTo( PdfObjectReference pageReference, PdfObjectId objectId )
            : base( objectId )
        {
            this[ PdfName.Names.Type ] = PdfName.Names.Action;
            this[ PdfName.Names.S ] = PdfName.Names.GoTo;
            this._pageReference = pageReference;
        }

        public PdfObjectReference PageReference
        {
            set { _pageReference = value; }
        }

        public int X
        {
            set { _xPosition = value / 1000m; }
        }

        public int Y
        {
            set { _yPosition = value / 1000m; }
        }

        public PdfObject GetAction()
        {
            return GetReference();
        }

        protected internal override void Write( PdfWriter writer )
        {
            var dest = new PdfArray();
            dest.Add( _pageReference );
            dest.Add( PdfName.Names.XYZ );
            dest.Add( new PdfNumeric( _xPosition ) );
            dest.Add( new PdfNumeric( _yPosition ) );
            dest.Add( PdfNull.Null );
            this[ PdfName.Names.D ] = dest;
            base.Write( writer );
        }
    }
}