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
        private PdfDictionary encrypt;

        // the documents idReferences
        private IDReferences idReferences;

        // The PDF information dictionary.
        private PdfInfo info;

        // the objects themselves
        // These objects are buffered and then written to the
        // PDF stream after each page has been rendered.  Adding
        // an object to this array does not mean it is ready for
        // writing, but it does mean that there is no need to 
        // wait until the end of the PDF stream.  The trigger
        // to write these objects out is pulled by PdfRenderer,
        // at the end of it's render page method.
        private readonly ArrayList objects = new ArrayList();

        // The root outline object
        private PdfOutline outlineRoot;

        // the /Resources object
        private readonly PdfResources resources;

        // list of objects to write in the trailer.
        private readonly ArrayList trailerObjects = new ArrayList();

        // the XObjects Map.
        private readonly Hashtable xObjectsMap = new Hashtable();

        // The cross-reference table.
        private readonly XRefTable xrefTable;

        // Links wiating for internal document references
        //private ArrayList pendingLinks;

        public PdfCreator( Stream stream )
        {
            // Create the underlying PDF document.
            Doc = new PdfDocument( stream );
            Doc.Version = PdfVersion.V13;

            resources = new PdfResources( Doc.NextObjectId() );
            addTrailerObject( resources );
            xrefTable = new XRefTable();
        }

        public PdfDocument Doc { get; private set; }

        public void setIDReferences( IDReferences idReferences )
        {
            this.idReferences = idReferences;
        }

        public void AddObject( PdfObject obj )
        {
            objects.Add( obj );
        }

        public PdfXObject AddImage( FonetImage img )
        {
            // check if already created
            string url = img.Uri;
            var xObject = (PdfXObject)xObjectsMap[ url ];
            if ( xObject == null )
            {
                PdfICCStream iccStream = null;

                ColorSpace cs = img.ColorSpace;
                if ( cs.HasICCProfile() )
                {
                    iccStream = new PdfICCStream( Doc.NextObjectId(), cs.GetICCProfile() );
                    iccStream.NumComponents = new PdfNumeric( cs.GetNumComponents() );
                    iccStream.AddFilter( new FlateFilter() );
                    objects.Add( iccStream );
                }

                // else, create a new one
                var name = new PdfName( "XO" + xObjectsMap.Count );
                xObject = new PdfXObject( img.Bitmaps, name, Doc.NextObjectId() );
                xObject.SubType = PdfName.Names.Image;
                xObject.Dictionary[ PdfName.Names.Width ] = new PdfNumeric( img.Width );
                xObject.Dictionary[ PdfName.Names.Height ] = new PdfNumeric( img.Height );
                xObject.Dictionary[ PdfName.Names.BitsPerComponent ] = new PdfNumeric( img.BitsPerPixel );

                // Check for ICC color space
                if ( iccStream != null )
                {
                    var ar = new PdfArray();
                    ar.Add( PdfName.Names.ICCBased );
                    ar.Add( iccStream.GetReference() );

                    xObject.Dictionary[ PdfName.Names.ColorSpace ] = ar;
                }
                else
                {
                    xObject.Dictionary[ PdfName.Names.ColorSpace ] =
                        new PdfName( img.ColorSpace.GetColorSpacePDFString() );
                }

                xObject.AddFilter( img.Filter );

                objects.Add( xObject );
                xObjectsMap.Add( url, xObject );
            }
            return xObject;
        }

        public PdfPage makePage( PdfResources resources, PdfContentStream contents,
            int pagewidth, int pageheight, Page currentPage )
        {
            var page = new PdfPage(
                resources, contents,
                pagewidth, pageheight,
                Doc.NextObjectId() );

            if ( currentPage != null )
            {
                foreach ( string id in currentPage.getIDList() )
                    idReferences.setInternalGoToPageReference( id, page.GetReference() );
            }

            /* add it to the list of objects */
            objects.Add( page );

            page.SetParent( Doc.Pages );
            Doc.Pages.Kids.Add( page.GetReference() );

            return page;
        }

        public PdfLink makeLink( Rectangle rect, string destination, int linkType )
        {
            var link = new PdfLink( Doc.NextObjectId(), rect );
            objects.Add( link );

            if ( linkType == LinkSet.EXTERNAL )
            {
                if ( destination.EndsWith( ".pdf" ) )
                {
                    // FileSpec
                    var fileSpec = new PdfFileSpec( Doc.NextObjectId(), destination );
                    objects.Add( fileSpec );
                    var gotoR = new PdfGoToRemote( fileSpec, Doc.NextObjectId() );
                    objects.Add( gotoR );
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
                PdfObjectReference goToReference = getGoToReference( destination );
                var internalLink = new PdfInternalLink( goToReference );
                link.SetAction( internalLink );
            }
            return link;
        }

        private PdfObjectReference getGoToReference( string destination )
        {
            PdfGoTo goTo;
            // Have we seen this 'id' in the document yet?
            if ( idReferences.doesIDExist( destination ) )
            {
                if ( idReferences.doesGoToReferenceExist( destination ) )
                    goTo = idReferences.getInternalLinkGoTo( destination );
                else
                {
                    goTo = idReferences.createInternalLinkGoTo( destination, Doc.NextObjectId() );
                    addTrailerObject( goTo );
                }
            }
            else
            {
                // id was not found, so create it
                idReferences.CreateUnvalidatedID( destination );
                idReferences.AddToIdValidationList( destination );
                goTo = idReferences.createInternalLinkGoTo( destination, Doc.NextObjectId() );
                addTrailerObject( goTo );
            }
            return goTo.GetReference();
        }

        private void addTrailerObject( PdfObject obj )
        {
            trailerObjects.Add( obj );
        }

        public PdfContentStream makeContentStream()
        {
            var obj = new PdfContentStream( Doc.NextObjectId() );
            obj.AddFilter( new FlateFilter() );
            objects.Add( obj );
            return obj;
        }

        public PdfAnnotList makeAnnotList()
        {
            var obj = new PdfAnnotList( Doc.NextObjectId() );
            objects.Add( obj );
            return obj;
        }

        public void SetOptions( PdfRendererOptions options )
        {
            // Configure the /Info dictionary.
            info = new PdfInfo( Doc.NextObjectId() );
            if ( options.Title != null )
                info.Title = new PdfString( options.Title );
            if ( options.Author != null )
                info.Author = new PdfString( options.Author );
            if ( options.Subject != null )
                info.Subject = new PdfString( options.Subject );
            if ( options.Keywords != string.Empty )
                info.Keywords = new PdfString( options.Keywords );
            if ( options.Creator != null )
                info.Creator = new PdfString( options.Creator );
            if ( options.Producer != null )
                info.Producer = new PdfString( options.Producer );
            info.CreationDate = new PdfString( PdfDate.Format( DateTime.Now ) );
            objects.Add( info );

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
                encrypt = Doc.Writer.SecurityManager.GetEncrypt( Doc.NextObjectId() );
                objects.Add( encrypt );
            }
        }

        public PdfOutline getOutlineRoot()
        {
            if ( outlineRoot != null )
                return outlineRoot;

            outlineRoot = new PdfOutline( Doc.NextObjectId(), null, null );
            addTrailerObject( outlineRoot );
            Doc.Catalog.Outlines = outlineRoot;
            return outlineRoot;
        }

        public PdfOutline makeOutline( PdfOutline parent, string label,
            string destination )
        {
            PdfObjectReference goToRef = getGoToReference( destination );

            var obj = new PdfOutline( Doc.NextObjectId(), label, goToRef );

            if ( parent != null )
                parent.AddOutline( obj );
            objects.Add( obj );
            return obj;
        }

        public PdfResources getResources()
        {
            return resources;
        }

        private void WritePdfObject( PdfObject obj )
        {
            xrefTable.Add( obj.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( obj );
        }

        public void output()
        {
            foreach ( PdfObject obj in objects )
                WritePdfObject( obj );
            objects.Clear();
        }

        public void outputHeader()
        {
            Doc.WriteHeader();
        }

        public void outputTrailer()
        {
            output();

            foreach ( PdfXObject xobj in xObjectsMap.Values )
                resources.AddXObject( xobj );

            xrefTable.Add( Doc.Catalog.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( Doc.Catalog );

            xrefTable.Add( Doc.Pages.ObjectId, Doc.Writer.Position );
            Doc.Writer.WriteLine( Doc.Pages );

            foreach ( PdfObject o in trailerObjects )
                WritePdfObject( o );

            // output the xref table
            long xrefOffset = Doc.Writer.Position;
            xrefTable.Write( Doc.Writer );

            // output the file trailer
            var trailer = new PdfFileTrailer();
            trailer.Size = new PdfNumeric( Doc.ObjectCount + 1 );
            trailer.Root = Doc.Catalog.GetReference();
            trailer.Id = Doc.FileIdentifier;
            if ( info != null )
                trailer.Info = info.GetReference();
            if ( info != null && encrypt != null )
                trailer.Encrypt = encrypt.GetReference();
            trailer.XRefOffset = xrefOffset;
            Doc.Writer.Write( trailer );
        }
    }
}