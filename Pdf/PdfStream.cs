using System;
using System.Collections;
using Fonet.Pdf.Filter;
using Fonet.Pdf.Security;

namespace Fonet.Pdf
{
    public class PdfStream : PdfObject
    {
        protected byte[] Data;

        private readonly PdfDictionary _dictionary = new PdfDictionary();

        private IList _filters;

        public PdfStream()
        {
        }

        public PdfStream( PdfObjectId objectId ) : base( objectId )
        {
        }

        public PdfStream( byte[] data )
        {
            this.Data = data;
        }

        public PdfStream( byte[] data, PdfObjectId objectId )
            : base( objectId )
        {
            this.Data = data;
        }

        private PdfObject FilterName
        {
            get
            {
                if ( !HasFilters )
                    return PdfNull.Null;
                if ( _filters.Count == 1 )
                {
                    var filter = (IFilter)_filters[ 0 ];
                    return filter.Name;
                }
                var names = new PdfArray();
                foreach ( IFilter filter in _filters )
                    names.Add( filter.Name );
                return names;
            }
        }

        private PdfObject FilterDecodeParms
        {
            get
            {
                if ( !HasFilters )
                    return PdfNull.Null;
                if ( _filters.Count == 1 )
                {
                    var filter = (IFilter)_filters[ 0 ];
                    return filter.DecodeParms;
                }
                var decodeParams = new PdfArray();
                foreach ( IFilter filter in _filters )
                    decodeParams.Add( filter.DecodeParms );
                return decodeParams;
            }
        }

        private bool HasFilters
        {
            get
            {
                if ( _filters != null )
                    return _filters.Count > 0;
                return false;
            }
        }

        private bool HasDecodeParams
        {
            get
            {
                if ( _filters == null )
                    return false;
                foreach ( IFilter filter in _filters )
                {
                    if ( filter.HasDecodeParams )
                        return true;
                }
                return false;
            }
        }

        public PdfDictionary Dictionary
        {
            get { return _dictionary; }
        }

        public void AddFilter( IFilter filter )
        {
            if ( filter == null )
                throw new ArgumentNullException( "filter" );
            if ( _filters == null )
                _filters = new ArrayList();
            _filters.Add( filter );
        }

        private byte[] ApplyFilters( byte[] data )
        {
            if ( _filters == null )
                return data;

            byte[] encoded = data;
            for ( int x = _filters.Count - 1; x >= 0; x-- )
            {
                var filter = (IFilter)_filters[ x ];
                encoded = filter.Encode( encoded );
            }
            return encoded;
        }

        protected internal override void Write( PdfWriter writer )
        {
            if ( writer == null )
                throw new ArgumentNullException( "writer" );
            if ( Data == null )
                throw new InvalidOperationException( "No data for stream." );

            // Prepare the stream's data.
            var bytes = (byte[])Data.Clone();

            // Apply any filters.
            if ( HasFilters )
                bytes = ApplyFilters( Data );

            // Encrypt the data if required.
            SecurityManager sm = writer.SecurityManager;
            if ( sm != null )
                bytes = sm.Encrypt( bytes, writer.EnclosingIndirect.ObjectId );

            // Create the stream's dictionary.
            _dictionary[ PdfName.Names.Length ] = new PdfNumeric( bytes.Length );
            if ( HasFilters )
            {
                _dictionary[ PdfName.Names.Filter ] = FilterName;
                if ( HasDecodeParams )
                    _dictionary[ PdfName.Names.DecodeParams ] = FilterDecodeParms;
            }

            // Write out the dictionary.
            writer.WriteLine( _dictionary );

            // Write out the stream data.
            writer.WriteKeywordLine( Keyword.Stream );
            writer.WriteLine( bytes );
            writer.WriteKeyword( Keyword.EndStream );
        }
    }
}