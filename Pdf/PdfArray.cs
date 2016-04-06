using System;
using System.Collections;

namespace Fonet.Pdf
{
    public class PdfArray : PdfObject, IEnumerable
    {
        private readonly ArrayList _elements = new ArrayList();

        public PdfArray()
        {
        }

        public PdfArray( PdfObjectId objectId ) : base( objectId )
        {
        }

        public PdfObject this[ int index ]
        {
            get { return (PdfObject)_elements[ index ]; }
            set { _elements[ index ] = value; }
        }

        public int Count
        {
            get { return _elements.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        public int Add( PdfObject value )
        {
            return _elements.Add( value );
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains( PdfObject value )
        {
            return _elements.Contains( value );
        }

        public int IndexOf( PdfObject value )
        {
            return _elements.IndexOf( value );
        }

        public void Insert( int index, PdfObject value )
        {
            _elements.Insert( index, value );
        }

        public void Remove( PdfObject value )
        {
            _elements.Remove( value );
        }

        public void RemoveAt( int index )
        {
            _elements.RemoveAt( index );
        }

        public void AddArray( Array data )
        {
            foreach ( object entry in data )
                Add( new PdfNumeric( Convert.ToDecimal( entry ) ) );
        }

        protected internal override void Write( PdfWriter writer )
        {
            writer.WriteKeyword( Keyword.ArrayBegin );
            var isFirst = true;
            foreach ( PdfObject obj in _elements )
            {
                if ( !isFirst )
                    writer.WriteSpace();
                else
                    isFirst = false;
                writer.Write( obj );
            }
            writer.WriteKeyword( Keyword.ArrayEnd );
        }
    }
}