using System;
using System.Collections;

namespace Fonet.Pdf
{
    public class PdfArray : PdfObject, IEnumerable
    {
        private readonly ArrayList elements = new ArrayList();

        public PdfArray()
        {
        }

        public PdfArray( PdfObjectId objectId ) : base( objectId )
        {
        }

        public PdfObject this[ int index ]
        {
            get { return (PdfObject)elements[ index ]; }
            set { elements[ index ] = value; }
        }

        public int Count
        {
            get { return elements.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public int Add( PdfObject value )
        {
            return elements.Add( value );
        }

        public void Clear()
        {
            elements.Clear();
        }

        public bool Contains( PdfObject value )
        {
            return elements.Contains( value );
        }

        public int IndexOf( PdfObject value )
        {
            return elements.IndexOf( value );
        }

        public void Insert( int index, PdfObject value )
        {
            elements.Insert( index, value );
        }

        public void Remove( PdfObject value )
        {
            elements.Remove( value );
        }

        public void RemoveAt( int index )
        {
            elements.RemoveAt( index );
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
            foreach ( PdfObject obj in elements )
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