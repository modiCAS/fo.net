using Fonet.Pdf;

namespace Fonet.DataTypes
{
    internal class IDNode
    {
        private readonly string _idValue;

        private PdfGoTo _internalLinkGoTo;

        private PdfObjectReference _internalLinkGoToPageReference;

        private int _pageNumber = -1;
        private int _xPosition;
        private int _yPosition;

        internal IDNode( string idValue )
        {
            this._idValue = idValue;
        }

        internal void SetPageNumber( int number )
        {
            _pageNumber = number;
        }

        public string GetPageNumber()
        {
            return _pageNumber != -1 ? _pageNumber.ToString() : null;
        }

        internal void CreateInternalLinkGoTo( PdfObjectId objectId )
        {
            if ( _internalLinkGoToPageReference == null )
                _internalLinkGoTo = new PdfGoTo( null, objectId );
            else
                _internalLinkGoTo = new PdfGoTo( _internalLinkGoToPageReference, objectId );

            if ( _xPosition != 0 )
            {
                _internalLinkGoTo.X = _xPosition;
                _internalLinkGoTo.Y = _yPosition;
            }
        }

        internal void SetInternalLinkGoToPageReference( PdfObjectReference pageReference )
        {
            if ( _internalLinkGoTo != null )
                _internalLinkGoTo.PageReference = pageReference;
            else
                _internalLinkGoToPageReference = pageReference;
        }

        internal string GetInternalLinkGoToReference()
        {
            return _internalLinkGoTo.ObjectId.ObjectNumber + " " + _internalLinkGoTo.ObjectId.GenerationNumber + " R";
        }

        protected string GetIDValue()
        {
            return _idValue;
        }

        internal PdfGoTo GetInternalLinkGoTo()
        {
            return _internalLinkGoTo;
        }

        internal bool IsThereInternalLinkGoTo()
        {
            return _internalLinkGoTo != null;
        }

        internal void SetPosition( int x, int y )
        {
            if ( _internalLinkGoTo != null )
            {
                _internalLinkGoTo.X = x;
                _internalLinkGoTo.Y = y;
            }
            else
            {
                _xPosition = x;
                _yPosition = y;
            }
        }
    }
}