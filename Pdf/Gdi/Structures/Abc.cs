using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi.Structures
{
    /// <summary>
    ///     The ABC structure contains the width of a character in a TrueType font.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct Abc
    {
        public int AbcA;
        public uint AbcB;
        public int AbcC;
    }
}