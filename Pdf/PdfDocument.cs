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
        private FileIdentifier _fileId = new FileIdentifier();

        private uint _nextObjectNumber = 1;

        private PdfVersion _version = PdfVersion.V14;

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
            get { return _version; }
            set { _version = value; }
        }

        public FileIdentifier FileIdentifier
        {
            get { return _fileId; }
            set { _fileId = value; }
        }

        public SecurityOptions SecurityOptions
        {
            set { Writer.SecurityManager = new SecurityManager( value, _fileId ); }
        }

        public PdfCatalog Catalog { get; private set; }

        public PdfPageTree Pages { get; private set; }

        public uint ObjectCount
        {
            get { return _nextObjectNumber - 1; }
        }

        public PdfWriter Writer { get; private set; }

        public PdfObjectId NextObjectId()
        {
            return new PdfObjectId( _nextObjectNumber++, 0 );
        }

        public void WriteHeader()
        {
            Writer.WriteHeader( _version );
            Writer.WriteBinaryComment();
        }
    }
}