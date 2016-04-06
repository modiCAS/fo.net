namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the Control Value Program table ('prep').
    /// </summary>
    internal class ControlValueProgramTable : FontTable
    {
        /// <summary>
        ///     Set of instructions executed whenever point size or font
        ///     or transformation change.
        /// </summary>
        private byte[] _instructions;

        /// <summary>
        ///     Creates an instance of the <see cref="ControlValueProgramTable" /> class.
        /// </summary>
        /// <param name="entry"></param>
        public ControlValueProgramTable( DirectoryEntry entry )
            : base( TableNames.Prep, entry )
        {
        }

        /// <summary>
        ///     Reads the contents of the "prep" table from the current position
        ///     in the supplied stream.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            _instructions = new byte[ Entry.Length ];
            reader.Stream.Read( _instructions, 0, _instructions.Length );
        }

        /// <summary>
        ///     Writes out the array of instructions to the supplied stream.
        /// </summary>
        /// <param name="writer"></param>
        protected internal override void Write( FontFileWriter writer )
        {
            writer.Stream.Write( _instructions, 0, _instructions.Length );
        }
    }
}