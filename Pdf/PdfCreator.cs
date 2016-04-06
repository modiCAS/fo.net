using System;
using System.Collections;
using System.Drawing;
using System.IO;
using Fonet.DataTypes;
using Fonet.Image;
using Fonet.Layout;
using Fonet.Pdf.Filter;
using Fonet.Pdf.Security;
using Fonet.Render.Pdf;

namespace Fonet.Pdf
{
    internal sealed class PdfCreator
    {
        // The PDF encryption dictionary.
        private PdfDictionary _encrypt;

        // the documents idReferences
        private IDReferences _idReferences;

        // The PDF information dictionary.
        private PdfInfo _info;

        // the objects themselves
        // These objects are buffered and then written to the
        // PDF stream after each page has been rendered.  Adding
        // an object to this array does not mean it is ready for
        // writing, but it does mean that there is no need to 
        // wait until the end of the PDF stream.  The trigger
        // to write these objects out is pulled by PdfRenderer,
        // at the end of it's render page method.
        private readonly ArrayList _objects = new ArrayList();

        // The root outline object
        private PdfOutline _outlineRoot;

        // the /Resources object
        private readonly PdfResources _resources;

        // list of objects to write in the trailer.
        private readonly ArrayList _trailerObjects = new ArrayList();

        // the XObjects Map.
        private readonly Hashtable _xObjectsMap = new Hashtable();

        // The cross-reference table.
        private readonly XRefTable _xrefTable;

        // Links wiating for internal document references
        //private ArrayList pendingLinks;

        public PdfCreator( Stream stream )
        {
            // Create the underlying PDF document.
            Doc = new PdfDocument( stream );
            Doc.Version = PdfVersion.V13;

            _resources = new PdfResources( Doc.NextObjectId() );
            AddTrailerObject( _resources );
            _xrefTable = new XRefTable();
        }

        public PdfDocument Doc { get; private set; }

        public void SetIDReferences( IDReferences idReferences )
        {
            this._idReferences = idReferences;
        }

        public void AddObject( PdfObject obj )
        {
            _objects.Add( obj );
        }

        public PdfXObject AddImage( FonetImage img )
        {
            // check if already created
            string url = img.Uri;
            var xObject = (PdfXObject)_xObjectsMap[ url ];
            if ( xObject == null )
            {
                PdfIccStream iccStream = null;

                ColorSpace cs = img.ColorSpace;
                if ( cs.HasIccProfile() )
                {
                    iccStream = new PdfIccStream( Doc.NextObjectId(), cs.GetIccProfile() );
                    iccStream.NumComponents = new PdfNumeric( cs.GetNumComponents() );
                    iccStream.AddFilter( new FlateFilter() );
                    _objects.Add( iccStream );
                }

                // else, create a new one
                var name = new PdfName( "XO" + _xObjectsMap.Count );
                xObject = new PdfXObject( img.Bitmaps, name, Doc.NextObjectId() );
                xObject.SubType = PdfName.Names.Image;
                xObject.Dictionary[ PdfName.Names.Width ] = new PdfNumeric( img.Width );
                xObject.Dictionary[ PdfName.Names.Height ] = new PdfNumeric( img.Height );
                xObject.Dictionary[ PdfName.Names.BitsPerComponent ] = new PdfNumeric( img.BitsPerPixel );

                // Check for ICC color space
                if ( iccStream != null )
                {
                    var ar = new PdfArray();
                    ar.Add( PdfName.Names.IccBased );
                    ar.Add( iccStream.GetReference() );

                    xObject.Dictionary[ PdfName.Names.ColorSpace ] = ar;
                }
                else
                {
                    xObject.Dictionary[ PdfName.Names.ColorSpace ] =
                        new PdfName( img.ColorSpace.GetColorSpacePdfString() );
                }

                xObject.AddFilter( img.Filter );

                _objects.Add( xObject );
                _xObjectsMap.Add( url, xObject );
            }
            return xObject;
        }

        public PdfPage MakePage( PdfResources resources, PdfContentStream contents,
            int pagewidth, int pageheight, Page currentPage )
        {
            var page = new PdfPage(
                resources, contents,
                pagewidth, pageheight,
                Doc.NextObjectId() );

            if ( currentPage != null )
            {
                foreach ( string id in currentPage.getIDList() )
                    _idReferences.SetInternalGoToPageReference( id, page.GetReference() );
            }

            /* add it to the list of objects */
            _objects.Add( page );

            page.SetParent( Doc.Pages );
            Doc.Pages.Kids.Add( page.GetReference() );

            return page;
        }

        public PdfLink MakeLink( Rectangle rect, string destination, int linkType )
        {
            var link = new PdfLink( Doc.NextObjectId(), rect );
            _objects.Add( link );

            if ( linkType == LinkSet.EXTERNAL )
            {
                if ( destination.EndsWith( ".pdf" ) )
                {
                    // FileSpec
                    var fileSpec = new PdfFileSpec( Doc.NextObjectId(), destination );
                    _objects.Add( fileSpec );
                    var gotoR = new PdfGoToRemote( fileSpec, Doc.NextObjectId() );
                    _objects.Add( gotoR );
                    link.SetAction( gotoR );
                }
                else
                {
                    // URI
                    var uri = new PdfUri( destination );
                    link.SetAction( uri );
                }
            }
            else
            {
                PdfObjectReference goToReference = GetGoToReference( destination );
                var internalLink = new PdfInternalLink( goToReference );
                link.SetAction( internalLink );
            }
            return link;
        }

        private PdfObjectReference GetGoToReference( string destination )
        {
            PdfGoTo goTo;
            // Have we seen this 'id' in the document yet?
            if ( _idReferences.DoesIDExist( destination ) )
            {
                if ( _idReferences.DoesGoToReferenceExist( destination ) )
                    goTo = _idReferences.GetInternalLinkGoTo( destination );
                else
                {
                    goTo = _idReferences.CreateInternalLinkGoTo( destination, Doc.NextObjectId() );
                    AddTrailerObject( goTo );
                }
            }
            else
            {
                // id was not found, so create it
                _idReferences.CreateUnvalidatedID( destination );
                _idReferences.AddToIdValidationList( destination );
                goTo = _idReferences.CreateInternalLinkGoTo( destination, Doc.NextObjectId() );
                AddTrailerObject( goTo );
            }
            return goTo.GetReference();
        }

        private void AddTrailerObject( PdfObject obj )
        {
            _trailerObjects.Add( obj );
        }

        public PdfContentStream MakeContentStream()
        {
            var obj = new PdfContentStream( Doc.NextObjectId() );
            obj.AddFilter( new FlateFilter() );
            _objects.Add( obj );
            return obj;
        }

        public PdfAnnotList MakeAnnotList()
        {
            var obj = new PdfAnnotList( Doc.NextObjectId() );
            _objects.Add( obj );
            return obj;
        }

        public void SetOptions( PdfRendererOptions options )
        {
            // Configure the /Info dictionary.
            _info = new PdfInfo( Doc.NextObjectId() );
            if ( options.Title != null )
                _info.Title = new PdfString( options.Title );
            if ( options.Author != null )
                _info.Author = new PdfString( options.Author );
            if ( options.Subject != null )
                _info.Subject = new PdfString( options.Subject );
            if ( options.Keywords != string.Empty )
                _info.Keywords = new PdfString( options.Keywords );
            if ( options.Creator != null )
                _info.Creator = new PdfString( options.Creator );
            if ( options.Producer != null )
                _info.Producer = new PdfString( options.Producer );
            _info.CreationDate = new PdfString( PdfDate.Format( DateTime.Now ) );
            _objects.Add( _info );

            // Configure the security options.
            if ( options.UserPassword != null ||
                options.OwnerPassword != null ||
                options.HasPermissions )
            {
                var securityOptions = new SecurityOptions();
                securityOptions.UserPassword = options.UserPassword;
                securityOptions.OwnerPassword = options.OwnerPassword;
                securityOptions.EnableAdding( options.EnableAdd );
                securityOptions.EnableChanging( options.EnableModify );
                securityOptions.EnableCopying( options.EnableCopy );
                securityOptions.EnablePrinting( options.EnablePrinting );

                Doc.SecurityOptions = securityOptions;
                _encrypt = Doc.Writer.SecurityManager.GetEncrypt( Doc.NextObjectId() );
                _objects.Add( _encrypt );
            }
        }

        public PdfOutline GetOutlineRoot()
        {
            if ( _outlineRoot != null )
                return _outlineRoot;

            _outlineRoot = new PdfOutline( Doc.NextObjectId(), null, null );
            AddTrailerObject( _outlineRoot );
            Doc.Catalog.Outlines = _outlineRoot;
            return _outlineRoot;
        }

        public PdfOutline MakeOutline( PdfOutline parent, string label,
            string destination )
        {
            PdfObjectReference goToRef = GetGoToReference( destination );

            var obj = new PdfOutline( Doc.NextObjectId(), label, goToRef );

            if ( parent != null )
                parent.AddOutline( obj );
            _objects.Add( obj );
            return obj;
        }

        public PdfResources GetResources()
        {
            return _resources;
        }

        private void WritePdfObject( PdfObject obj )
        {
            _xrefTable.Add( obj.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( obj );
        }

        public void Output()
        {
            foreach ( PdfObject obj in _objects )
                WritePdfObject( obj );
            _objects.Clear();
        }

        public void OutputHeader()
        {
            Doc.WriteHeader();
        }

        public void OutputTrailer()
        {
            Output();

            foreach ( PdfXObject xobj in _xObjectsMap.Values )
                _resources.AddXObject( xobj );

            _xrefTable.Add( Doc.Catalog.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( Doc.Catalog );

            _xrefTable.Add( Doc.Pages.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( Doc.Pages );

            foreach ( PdfObject o in _trailerObjects )
                WritePdfObject( o );

            // output the xref table
            long xrefOffset = Doc.Writer.Position;
            _xrefTable.Write( Doc.Writer );

            // output the file trailer
            var trailer = new PdfFileTrailer();
            trailer.Size = new PdfNumeric( Doc.ObjectCount + 1 );
            trailer.Root = Doc.Catalog.GetReference();
            trailer.Id = Doc.FileIdentifier;
            if ( _info != null )
                trailer.Info = _info.GetReference();
            if ( _info != null && _encrypt != null )
                trailer.Encrypt = _encrypt.GetReference();
            trailer.XRefOffset = xrefOffset;
            Doc.Writer.Write( trailer );
        }
    }
}