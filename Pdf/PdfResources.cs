namespace Fonet.Pdf
{
    public class PdfResources : PdfDictionary
    {
        private static readonly PdfArray DefaultProcedureSets;

        private readonly PdfDictionary _fonts = new PdfDictionary();

        private readonly PdfDictionary _xObjects = new PdfDictionary();

        static PdfResources()
        {
            DefaultProcedureSets = new PdfArray();
            DefaultProcedureSets.Add( PdfName.Names.Pdf );
            DefaultProcedureSets.Add( PdfName.Names.Text );
            DefaultProcedureSets.Add( PdfName.Names.ImageB );
            DefaultProcedureSets.Add( PdfName.Names.ImageC );
            DefaultProcedureSets.Add( PdfName.Names.ImageI );
        }

        public PdfResources( PdfObjectId objectId )
            : base( objectId )
        {
            this[ PdfName.Names.ProcSet ] = DefaultProcedureSets;
        }

        public void AddFont( PdfFont font )
        {
            _fonts.Add( font.Name, font.GetReference() );
        }

        public void AddXObject( PdfXObject xObject )
        {
            _xObjects.Add( xObject.Name, xObject.GetReference() );
        }

        protected internal override void Write( PdfWriter writer )
        {
            if ( _fonts.Count > 0 )
                this[ PdfName.Names.Font ] = _fonts;
            if ( _xObjects.Count > 0 )
                this[ PdfName.Names.XObject ] = _xObjects;
            base.Write( writer );
        }
    }
}