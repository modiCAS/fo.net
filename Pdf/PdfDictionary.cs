using System;
using System.Collections;

namespace Fonet.Pdf
{
    public class PdfDictionary : PdfObject, IEnumerable
    {
        protected Hashtable Entries = new Hashtable();

        public PdfDictionary()
        {
        }

        public PdfDictionary( PdfObjectId objectId )
            : base( objectId )
        {
        }

        public PdfObject this[ PdfName key ]
        {
            get
            {
                if ( key == null )
                    throw new ArgumentNullException( "key" );
                return (PdfObject)Entries[ key ];
            }
            set
            {
                if ( key == null )
                    throw new ArgumentNullException( "key" );
                Entries[ key ] = value;
            }
        }

        public ICollection Keys
        {
            get { return Entries.Keys; }
        }

        public ICollection Values
        {
            get { return Entries.Values; }
        }

        public int Count
        {
            get { return Entries.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        public void Add( PdfName key, PdfObject value )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );
            if ( Entries.ContainsKey( key ) )
                throw new ArgumentException( "Already contains entry " + key );

            Entries.Add( key, value );
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public bool Contains( PdfName key )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );
            return Entries.ContainsKey( key );
        }

        public void Remove( PdfName key )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );
            Entries.Remove( key );
        }

        protected internal override void Write( PdfWriter writer )
        {
            writer.WriteKeywordLine( Keyword.DictionaryBegin );
            foreach ( DictionaryEntry e in Entries )
            {
                writer.Write( (PdfName)e.Key );
                writer.WriteSpace();
                writer.WriteLine( (PdfObject)e.Value );
            }
            writer.WriteKeyword( Keyword.DictionaryEnd );
        }
    }
}