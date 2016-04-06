using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Fonet.Pdf
{
    public class PdfContentStream : PdfStream
    {
        protected MemoryStream Stream;
        protected PdfWriter StreamData;

        public PdfContentStream( PdfObjectId objectId )
            : base( objectId )
        {
            Stream = new MemoryStream();
            StreamData = new PdfWriter( Stream );
        }

        public void Write( PdfObject obj )
        {
            Debug.Assert( obj != null );
            if ( obj.IsIndirect || obj is PdfObjectReference )
                throw new ArgumentException( "Cannot write indirect PdfObject", "obj" );

            StreamData.Write( obj );
        }

        public void WriteLine( PdfObject obj )
        {
            Debug.Assert( obj != null );
            if ( obj.IsIndirect || obj is PdfObjectReference )
                throw new ArgumentException( "Cannot write indirect PdfObject", "obj" );

            StreamData.WriteLine( obj );
        }

        /// <summary>
        ///     TODO: This method is temporary.  I'm assuming that all string should
        ///     be represented as a PdfString object?
        /// </summary>
        /// <param name="s"></param>
        public void Write( string s )
        {
            StreamData.Write( Encoding.Default.GetBytes( s ) );
        }

        public void WriteLine( string s )
        {
            StreamData.WriteLine( Encoding.Default.GetBytes( s ) );
        }

        public void Write( int val )
        {
            StreamData.Write( val );
        }

        public void WriteLine( int val )
        {
            StreamData.WriteLine( val );
        }

        public void Write( decimal val )
        {
            StreamData.Write( val );
        }

        public void WriteLine( decimal val )
        {
            StreamData.WriteLine( val );
        }

        public void WriteSpace()
        {
            StreamData.WriteSpace();
        }

        public void WriteLine()
        {
            StreamData.WriteLine();
        }

        public void WriteByte( byte value )
        {
            StreamData.WriteByte( value );
        }

        public void Write( byte[] data )
        {
            StreamData.Write( data );
        }

        public void WriteKeyword( Keyword keyword )
        {
            StreamData.WriteKeyword( keyword );
        }

        public void WriteLine( byte[] data )
        {
            StreamData.WriteLine( data );
        }

        protected internal override void Write( PdfWriter writer )
        {
            Data = Stream.ToArray();
            base.Write( writer );
        }
    }
}