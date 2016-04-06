using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi.Structures
{
    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
    internal struct EnumLogFont
    {
        public LogFont elfLogFont;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
        public char[] elfFullName;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 64 )]
        public char[] elfStyle;
    }
}