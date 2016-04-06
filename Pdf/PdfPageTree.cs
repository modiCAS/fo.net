namespace Fonet.Pdf
{
    /// <summary>
    ///     The pages of a document are accessed through a structure known
    ///     as the page tree.
    /// </summary>
    /// <remarks>
    ///     The page tree is described in section 3.6.2 of the PDF specification.
    /// </remarks>
    public sealed class PdfPageTree : PdfDictionary
    {
        public PdfPageTree( PdfObjectId objectId )
            : base( objectId )
        {
            this[ PdfName.Names.Type ] = PdfName.Names.Pages;
            Kids = new PdfArray();
            this[ PdfName.Names.Kids ] = Kids;
        }

        public PdfArray Kids { get; private set; }

        protected internal override void Write( PdfWriter writer )
        {
            // Add a dictionary entry for /Count (the number of leaf 
            // nodes (page objects) that are descendants of this
            // node within the page tree.
            var count = 0;
            for ( var x = 0; x < Kids.Count; x++ )
                count++; // TODO: test if it is a leaf.
            this[ PdfName.Names.Count ] = new PdfNumeric( count );

            base.Write( writer );
        }
    }
}