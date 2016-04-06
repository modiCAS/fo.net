namespace Fonet.Pdf
{
    /// <summary>
    ///     Array class used to represent the /W entry in the CIDFont dictionary.
    /// </summary>
    public class PdfWArray : PdfObject
    {
        private readonly PdfArray _array = new PdfArray();
        private readonly int _startCid;

        public PdfWArray( int startCid )
        {
            this._startCid = startCid;
        }

        public void AddEntry( int[] widths )
        {
            _array.AddArray( widths );
        }

        protected internal override void Write( PdfWriter writer )
        {
            writer.WriteKeyword( Keyword.ArrayBegin );
            writer.WriteSpace();
            writer.Write( _startCid );
            writer.WriteSpace();
            _array.Write( writer );
            writer.WriteKeyword( Keyword.ArrayEnd );
        }
    }
}