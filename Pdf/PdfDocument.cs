using System.IO;
using Fonet.Pdf.Security;

namespace Fonet.Pdf
{
    /// <summary>
    ///     A class that enables a well structured PDF document to be generated.
    /// </summary>
    /// <remarks>
    ///     Responsible for allocating object identifiers.
    /// </remarks>
    public class PdfDocument
    {
        private FileIdentifier fileId = new FileIdentifier();

        private uint nextObjectNumber = 1;

        private PdfVersion version = PdfVersion.V14;

        public PdfDocument( Stream stream ) : this( new PdfWriter( stream ) )
        {
        }

        public PdfDocument( PdfWriter writer )
        {
            Writer = writer;
            Catalog = new PdfCatalog( NextObjectId() );
            Pages = new PdfPageTree( NextObjectId() );
            Catalog.Pages = Pages;
        }

        public PdfVersion Version
        {
            get { return version; }
            set { version = value; }
        }

        public FileIdentifier FileIdentifier
        {
            get { return fileId; }
            set { fileId = value; }
        }

        public SecurityOptions SecurityOptions
        {
            set { Writer.SecurityManager = new SecurityManager( value, fileId ); }
        }

        public PdfCatalog Catalog { get; private set; }

        public PdfPageTree Pages { get; private set; }

        public uint ObjectCount
        {
            get { return nextObjectNumber - 1; }
        }

        public PdfWriter Writer { get; private set; }

        public PdfObjectId NextObjectId()
        {
            return new PdfObjectId( nextObjectNumber++, 0 );
        }

        public void WriteHeader()
        {
            Writer.WriteHeader( version );
            Writer.WriteBinaryComment();
        }
    }
}