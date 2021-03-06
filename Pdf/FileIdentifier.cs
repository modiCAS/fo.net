using System;
using System.Text;

namespace Fonet.Pdf
{
    /// <summary>
    ///     A File Identifier is described in section 8.3 of the PDF specification.
    ///     The first string is a permanent identifier based on the contents of the file
    ///     at the time it was originally created, and does not change as the file is
    ///     incrementally updated.  The second string is a changing identifier based
    ///     on the file's contents the last time it was updated.
    /// </summary>
    /// <remarks>
    ///     If this class were being use to update a PDF's file identifier, we'd need
    ///     to add a method to parse an existing file identifier.
    /// </remarks>
    public class FileIdentifier : PdfObject
    {
        /// <summary>
        ///     Initialises the CreatedPart and ModifiedPart to a randomly generated GUID.
        /// </summary>
        public FileIdentifier()
        {
            string guid = Guid.NewGuid().ToString( "N" );
            // The GUID will only ever contain ASCII characters.
            CreatedPart = Encoding.ASCII.GetBytes( guid );
            ModifiedPart = (byte[])CreatedPart.Clone();
        }

        /// <summary>
        ///     Initialises the CreatedPart and ModifiedPart to the passed string.
        /// </summary>
        public FileIdentifier( byte[] createdPart )
        {
            CreatedPart = (byte[])createdPart.Clone();
            ModifiedPart = (byte[])createdPart.Clone();
        }

        /// <summary>
        ///     Returns the CreatedPart as a byte array.
        /// </summary>
        public byte[] CreatedPart { get; private set; }

        /// <summary>
        ///     Returns the ModifiedPart as a byte array.
        /// </summary>
        public byte[] ModifiedPart { get; private set; }

        protected internal override void Write( PdfWriter writer )
        {
            writer.WriteKeyword( Keyword.ArrayBegin );
            writer.Write( PdfString.ToPdfHexadecimal( new byte[] { }, CreatedPart ) );
            writer.WriteSpace();
            writer.Write( PdfString.ToPdfHexadecimal( new byte[] { }, ModifiedPart ) );
            writer.WriteKeyword( Keyword.ArrayEnd );
        }
    }
}