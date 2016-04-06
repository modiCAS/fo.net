namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the Font Program table ('fpgm').
    /// </summary>
    internal class FontProgramTable : FontTable
    {
        /// <summary>
        ///     List of N instructions.
        /// </summary>
        private byte[] _instructions;

        /// <summary>
        ///     Creates an instance of the <see cref="FontProgramTable" /> class.
        /// </summary>
        /// <param name="entry"></param>
        public FontProgramTable( DirectoryEntry entry )
            : base( TableNames.Fpgm, entry )
        {
        }

        /// <summary>
        ///     Gets the value representing the number of instructions
        ///     in the font program.
        /// </summary>
        public int Count
        {
            get { return _instructions.Length; }
        }

        /// <summary>
        ///     Reads the contents of the "fpgm" table from the current position
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