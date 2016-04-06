using System.Diagnostics;

namespace Fonet.Pdf
{
    public sealed class PdfObjectReference : PdfObject
    {
        private PdfObjectId _refId;

        public PdfObjectReference( PdfObject obj )
        {
            _refId = obj.ObjectId;
        }

        protected internal override void Write( PdfWriter writer )
        {
            Debug.Assert( !IsIndirect, "An object reference cannot be indirect" );

            writer.Write( _refId.ObjectNumber );
            writer.WriteSpace();
            writer.Write( _refId.GenerationNumber );
            writer.WriteSpace();
            writer.WriteKeyword( Keyword.R );
        }
    }
}