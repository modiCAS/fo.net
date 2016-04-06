using System.Collections;
using System.Text;
using Fonet.Layout;
using Fonet.Pdf;

namespace Fonet.DataTypes
{
    internal class IDReferences
    {
        private const int IDPadding = 5000;
        private readonly Hashtable _idReferences;
        private readonly Hashtable _idValidation;
        private readonly Hashtable _idUnvalidated;

        public IDReferences()
        {
            _idReferences = new Hashtable();
            _idValidation = new Hashtable();
            _idUnvalidated = new Hashtable();
        }

        public void InitializeID( string id, Area area )
        {
            CreateID( id );
            ConfigureID( id, area );
        }

        public void CreateID( string id )
        {
            if ( id != null && !id.Equals( "" ) )
            {
                if ( DoesUnvalidatedIDExist( id ) )
                {
                    RemoveFromUnvalidatedIDList( id );
                    RemoveFromIdValidationList( id );
                }
                else if ( DoesIDExist( id ) )
                {
                    throw new FonetException( "The id \"" + id
                        + "\" already exists in this document" );
                }
                else
                {
                    CreateNewId( id );
                    RemoveFromIdValidationList( id );
                }
            }
        }

        public void CreateUnvalidatedID( string id )
        {
            if ( id != null && !id.Equals( "" ) )
            {
                if ( !DoesIDExist( id ) )
                {
                    CreateNewId( id );
                    AddToUnvalidatedIdList( id );
                }
            }
        }

        public void AddToUnvalidatedIdList( string id )
        {
            _idUnvalidated[ id ] = "";
        }

        public void RemoveFromUnvalidatedIDList( string id )
        {
            _idUnvalidated.Remove( id );
        }

        public bool DoesUnvalidatedIDExist( string id )
        {
            return _idUnvalidated.ContainsKey( id );
        }

        public void ConfigureID( string id, Area area )
        {
            if ( id != null && !id.Equals( "" ) )
            {
                SetPosition( id,
                    area.getPage().getBody().getXPosition()
                        + area.getTableCellXOffset() - IDPadding,
                    area.getPage().getBody().GetYPosition()
                        - area.getAbsoluteHeight() + IDPadding );
                SetPageNumber( id, area.getPage().getNumber() );
                area.getPage().addToIDList( id );
            }
        }

        public void AddToIdValidationList( string id )
        {
            _idValidation[ id ] = "";
        }

        public void RemoveFromIdValidationList( string id )
        {
            _idValidation.Remove( id );
        }

        public void RemoveID( string id )
        {
            _idReferences.Remove( id );
        }

        public bool IsEveryIdValid()
        {
            return _idValidation.Count == 0;
        }

        public string GetInvalidIds()
        {
            var list = new StringBuilder();
            foreach ( object o in _idValidation.Keys )
            {
                list.Append( "\n\"" );
                list.Append( o );
                list.Append( "\" " );
            }
            return list.ToString();
        }

        public bool DoesIDExist( string id )
        {
            return _idReferences.ContainsKey( id );
        }

        public bool DoesGoToReferenceExist( string id )
        {
            var node = (IDNode)_idReferences[ id ];
            return node.IsThereInternalLinkGoTo();
        }

        public PdfGoTo GetInternalLinkGoTo( string id )
        {
            var node = (IDNode)_idReferences[ id ];
            return node.GetInternalLinkGoTo();
        }

        public PdfGoTo CreateInternalLinkGoTo( string id, PdfObjectId objectId )
        {
            var node = (IDNode)_idReferences[ id ];
            node.CreateInternalLinkGoTo( objectId );
            return node.GetInternalLinkGoTo();
        }

        public void CreateNewId( string id )
        {
            var node = new IDNode( id );
            _idReferences[ id ] = node;
        }

        public PdfGoTo GetPdfGoTo( string id )
        {
            var node = (IDNode)_idReferences[ id ];
            return node.GetInternalLinkGoTo();
        }

        public void SetInternalGoToPageReference( string id,
            PdfObjectReference pageReference )
        {
            var node = (IDNode)_idReferences[ id ];
            if ( node != null )
                node.SetInternalLinkGoToPageReference( pageReference );
        }

        public void SetPageNumber( string id, int pageNumber )
        {
            var node = (IDNode)_idReferences[ id ];
            node.SetPageNumber( pageNumber );
        }

        public string GetPageNumber( string id )
        {
            if ( DoesIDExist( id ) )
            {
                var node = (IDNode)_idReferences[ id ];
                return node.GetPageNumber();
            }
            AddToIdValidationList( id );
            return null;
        }

        public void SetPosition( string id, int x, int y )
        {
            var node = (IDNode)_idReferences[ id ];
            node.SetPosition( x, y );
        }

        public ICollection GetInvalidElements()
        {
            return _idValidation.Keys;
        }
    }
}