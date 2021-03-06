using System;
using System.Collections;
using System.Text;

namespace Fonet.Pdf
{
    /// <summary>
    ///     A sub-section in a PDF file's cross-reference table.
    /// </summary>
    /// <remarks>
    ///     The cross-reference table is described in section 3.4.3 of
    ///     the PDF specification.
    /// </remarks>
    internal class XRefSubSection
    {
        /// <summary>
        ///     This entries contained in this subsection.
        /// </summary>
        private readonly ArrayList _entries;

        /// <summary>
        ///     Creates a new blank sub-section, that initially contains no entries.
        /// </summary>
        internal XRefSubSection()
        {
            _entries = new ArrayList();
        }

        /// <summary>
        ///     Adds an entry to the sub-section.
        /// </summary>
        internal void Add( PdfObjectId objectId, long offset )
        {
            _entries.Add( new Entry( objectId, offset ) );
        }

        /// <summary>
        ///     Writes the cross reference sub-section to the passed PDF writer.
        /// </summary>
        internal void Write( PdfWriter writer )
        {
            // The format of each cross-reference entry should occupy
            // exacty 20 ASCII bytes including the newline character.
            var entryFormat = "{0:0000000000} {1:00000} {2}";
            if ( writer.NewLine.Length == 1 )
            {
                // If the newline is configured as only a single character,
                // then we need to add an extra space to ensure the
                // resulting entry is always 20 bytes long.
                entryFormat += " ";
            }

            // Sort the entries.
            _entries.Sort();

            // Identify the first and last entries.
            uint first = 0;
            uint last = ( (Entry)_entries[ _entries.Count - 1 ] ).ObjectId.ObjectNumber;

            // Work out the number of entries based on the first and last object numbers.
            uint count = last - first + 1;

            // Output the first object number and the number of entries.
            writer.Write( first );
            writer.WriteSpace();
            writer.WriteLine( count );

            // Output the head of the linked list of free entries.
            // Right now, this implmentation does not support free
            // entries properly, so we just output an empty list and
            // hope that only contingous entries are ever added.
            byte[] bytes = Encoding.ASCII.GetBytes(
                string.Format( entryFormat, 0, 65535, "f" ) );
            writer.WriteLine( bytes );

            // Output each entry.
            foreach ( Entry entry in _entries )
            {
                bytes = Encoding.ASCII.GetBytes(
                    string.Format( entryFormat, entry.Offset,
                        entry.ObjectId.GenerationNumber, "n" ) );
                writer.WriteLine( bytes );
            }
        }

        /// <summary>
        ///     Structure representing a single cross-reference entry.
        /// </summary>
        private struct Entry : IComparable
        {
            internal Entry( PdfObjectId objectId, long offset )
            {
                this.ObjectId = objectId;
                this.Offset = offset;
            }

            /// <summary>
            ///     The object number and generation number.
            /// </summary>
            internal PdfObjectId ObjectId;

            /// <summary>
            ///     The number of bytes from the beginning of the file to
            ///     the beginning of the object.
            /// </summary>
            internal readonly long Offset;

            /// <summary>
            ///     Implementation of IComparable.
            /// </summary>
            public int CompareTo( object obj )
            {
                return ObjectId.ObjectNumber.CompareTo(
                    ( (Entry)obj ).ObjectId.ObjectNumber );
            }
        }
    }
}