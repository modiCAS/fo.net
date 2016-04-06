namespace Fonet.Render.Pdf.Fonts
{
    internal class Helvetica : Base14Font
    {
        private static readonly int[] CodePointWidths;

        private static readonly CodePointMapping DefaultMapping
            = CodePointMapping.GetMapping( "WinAnsiEncoding" );

        static Helvetica()
        {
            CodePointWidths = new int[ 256 ];
            CodePointWidths[ 0x0041 ] = 667;
            CodePointWidths[ 0x00C6 ] = 1000;
            CodePointWidths[ 0x00C1 ] = 667;
            CodePointWidths[ 0x00C2 ] = 667;
            CodePointWidths[ 0x00C4 ] = 667;
            CodePointWidths[ 0x00C0 ] = 667;
            CodePointWidths[ 0x00C5 ] = 667;
            CodePointWidths[ 0x00C3 ] = 667;
            CodePointWidths[ 0x0042 ] = 667;
            CodePointWidths[ 0x0043 ] = 722;
            CodePointWidths[ 0x00C7 ] = 722;
            CodePointWidths[ 0x0044 ] = 722;
            CodePointWidths[ 0x0045 ] = 667;
            CodePointWidths[ 0x00C9 ] = 667;
            CodePointWidths[ 0x00CA ] = 667;
            CodePointWidths[ 0x00CB ] = 667;
            CodePointWidths[ 0x00C8 ] = 667;
            CodePointWidths[ 0x00D0 ] = 722;
            CodePointWidths[ 0x0080 ] = 556;
            CodePointWidths[ 0x0046 ] = 611;
            CodePointWidths[ 0x0047 ] = 778;
            CodePointWidths[ 0x0048 ] = 722;
            CodePointWidths[ 0x0049 ] = 278;
            CodePointWidths[ 0x00CD ] = 278;
            CodePointWidths[ 0x00CE ] = 278;
            CodePointWidths[ 0x00CF ] = 278;
            CodePointWidths[ 0x00CC ] = 278;
            CodePointWidths[ 0x004A ] = 500;
            CodePointWidths[ 0x004B ] = 667;
            CodePointWidths[ 0x004C ] = 556;
            CodePointWidths[ 0x004D ] = 833;
            CodePointWidths[ 0x004E ] = 722;
            CodePointWidths[ 0x00D1 ] = 722;
            CodePointWidths[ 0x004F ] = 778;
            CodePointWidths[ 0x008C ] = 1000;
            CodePointWidths[ 0x00D3 ] = 778;
            CodePointWidths[ 0x00D4 ] = 778;
            CodePointWidths[ 0x00D6 ] = 778;
            CodePointWidths[ 0x00D2 ] = 778;
            CodePointWidths[ 0x00D8 ] = 778;
            CodePointWidths[ 0x00D5 ] = 778;
            CodePointWidths[ 0x0050 ] = 667;
            CodePointWidths[ 0x0051 ] = 778;
            CodePointWidths[ 0x0052 ] = 722;
            CodePointWidths[ 0x0053 ] = 667;
            CodePointWidths[ 0x008A ] = 667;
            CodePointWidths[ 0x0054 ] = 611;
            CodePointWidths[ 0x00DE ] = 667;
            CodePointWidths[ 0x0055 ] = 722;
            CodePointWidths[ 0x00DA ] = 722;
            CodePointWidths[ 0x00DB ] = 722;
            CodePointWidths[ 0x00DC ] = 722;
            CodePointWidths[ 0x00D9 ] = 722;
            CodePointWidths[ 0x0056 ] = 667;
            CodePointWidths[ 0x0057 ] = 944;
            CodePointWidths[ 0x0058 ] = 667;
            CodePointWidths[ 0x0059 ] = 667;
            CodePointWidths[ 0x00DD ] = 667;
            CodePointWidths[ 0x009F ] = 667;
            CodePointWidths[ 0x005A ] = 611;
            CodePointWidths[ 0x0061 ] = 556;
            CodePointWidths[ 0x00E1 ] = 556;
            CodePointWidths[ 0x00E2 ] = 556;
            CodePointWidths[ 0x00B4 ] = 333;
            CodePointWidths[ 0x00E4 ] = 556;
            CodePointWidths[ 0x00E6 ] = 889;
            CodePointWidths[ 0x00E0 ] = 556;
            CodePointWidths[ 0x0026 ] = 667;
            CodePointWidths[ 0x00E5 ] = 556;
            CodePointWidths[ 0x005E ] = 469;
            CodePointWidths[ 0x007E ] = 584;
            CodePointWidths[ 0x002A ] = 389;
            CodePointWidths[ 0x0040 ] = 1015;
            CodePointWidths[ 0x00E3 ] = 556;
            CodePointWidths[ 0x0062 ] = 556;
            CodePointWidths[ 0x005C ] = 278;
            CodePointWidths[ 0x007C ] = 260;
            CodePointWidths[ 0x007B ] = 334;
            CodePointWidths[ 0x007D ] = 334;
            CodePointWidths[ 0x005B ] = 278;
            CodePointWidths[ 0x005D ] = 278;
            CodePointWidths[ 0x00A6 ] = 260;
            CodePointWidths[ 0x0095 ] = 350;
            CodePointWidths[ 0x0063 ] = 500;
            CodePointWidths[ 0x00E7 ] = 500;
            CodePointWidths[ 0x00B8 ] = 333;
            CodePointWidths[ 0x00A2 ] = 556;
            CodePointWidths[ 0x0088 ] = 333;
            CodePointWidths[ 0x003A ] = 278;
            CodePointWidths[ 0x002C ] = 278;
            CodePointWidths[ 0x00A9 ] = 737;
            CodePointWidths[ 0x00A4 ] = 556;
            CodePointWidths[ 0x0064 ] = 556;
            CodePointWidths[ 0x0086 ] = 556;
            CodePointWidths[ 0x0087 ] = 556;
            CodePointWidths[ 0x00B0 ] = 400;
            CodePointWidths[ 0x00A8 ] = 333;
            CodePointWidths[ 0x00F7 ] = 584;
            CodePointWidths[ 0x0024 ] = 556;
            CodePointWidths[ 0x0065 ] = 556;
            CodePointWidths[ 0x00E9 ] = 556;
            CodePointWidths[ 0x00EA ] = 556;
            CodePointWidths[ 0x00EB ] = 556;
            CodePointWidths[ 0x00E8 ] = 556;
            CodePointWidths[ 0x0038 ] = 556;
            CodePointWidths[ 0x0085 ] = 1000;
            CodePointWidths[ 0x0097 ] = 1000;
            CodePointWidths[ 0x0096 ] = 556;
            CodePointWidths[ 0x003D ] = 584;
            CodePointWidths[ 0x00F0 ] = 556;
            CodePointWidths[ 0x0021 ] = 278;
            CodePointWidths[ 0x00A1 ] = 333;
            CodePointWidths[ 0x0066 ] = 278;
            CodePointWidths[ 0x0035 ] = 556;
            CodePointWidths[ 0x0083 ] = 556;
            CodePointWidths[ 0x0034 ] = 556;
            CodePointWidths[ 0xA4 ] = 167;
            CodePointWidths[ 0x0067 ] = 556;
            CodePointWidths[ 0x00DF ] = 611;
            CodePointWidths[ 0x0060 ] = 333;
            CodePointWidths[ 0x003E ] = 584;
            CodePointWidths[ 0x00AB ] = 556;
            CodePointWidths[ 0x00BB ] = 556;
            CodePointWidths[ 0x008B ] = 333;
            CodePointWidths[ 0x009B ] = 333;
            CodePointWidths[ 0x0068 ] = 556;
            CodePointWidths[ 0x002D ] = 333;
            CodePointWidths[ 0x0069 ] = 222;
            CodePointWidths[ 0x00ED ] = 278;
            CodePointWidths[ 0x00EE ] = 278;
            CodePointWidths[ 0x00EF ] = 278;
            CodePointWidths[ 0x00EC ] = 278;
            CodePointWidths[ 0x006A ] = 222;
            CodePointWidths[ 0x006B ] = 500;
            CodePointWidths[ 0x006C ] = 222;
            CodePointWidths[ 0x003C ] = 584;
            CodePointWidths[ 0x00AC ] = 584;
            CodePointWidths[ 0x006D ] = 833;
            CodePointWidths[ 0x00AF ] = 333;
            CodePointWidths[ 0x2D ] = 324;
            CodePointWidths[ 0x00B5 ] = 556;
            CodePointWidths[ 0x00D7 ] = 584;
            CodePointWidths[ 0x006E ] = 556;
            CodePointWidths[ 0x0039 ] = 556;
            CodePointWidths[ 0x00F1 ] = 556;
            CodePointWidths[ 0x0023 ] = 556;
            CodePointWidths[ 0x006F ] = 556;
            CodePointWidths[ 0x00F3 ] = 556;
            CodePointWidths[ 0x00F4 ] = 556;
            CodePointWidths[ 0x00F6 ] = 556;
            CodePointWidths[ 0x009C ] = 944;
            CodePointWidths[ 0x00F2 ] = 556;
            CodePointWidths[ 0x0031 ] = 556;
            CodePointWidths[ 0x00BD ] = 834;
            CodePointWidths[ 0x00BC ] = 834;
            CodePointWidths[ 0x00B9 ] = 333;
            CodePointWidths[ 0x00AA ] = 370;
            CodePointWidths[ 0x00BA ] = 365;
            CodePointWidths[ 0x00F8 ] = 611;
            CodePointWidths[ 0x00F5 ] = 556;
            CodePointWidths[ 0x0070 ] = 556;
            CodePointWidths[ 0x00B6 ] = 537;
            CodePointWidths[ 0x0028 ] = 333;
            CodePointWidths[ 0x0029 ] = 333;
            CodePointWidths[ 0x0025 ] = 889;
            CodePointWidths[ 0x002E ] = 278;
            CodePointWidths[ 0x00B7 ] = 278;
            CodePointWidths[ 0x0089 ] = 1000;
            CodePointWidths[ 0x002B ] = 584;
            CodePointWidths[ 0x00B1 ] = 584;
            CodePointWidths[ 0x0071 ] = 556;
            CodePointWidths[ 0x003F ] = 556;
            CodePointWidths[ 0x00BF ] = 611;
            CodePointWidths[ 0x0022 ] = 355;
            CodePointWidths[ 0x0084 ] = 333;
            CodePointWidths[ 0x0093 ] = 333;
            CodePointWidths[ 0x0094 ] = 333;
            CodePointWidths[ 0x0091 ] = 222;
            CodePointWidths[ 0x0092 ] = 222;
            CodePointWidths[ 0x0082 ] = 222;
            CodePointWidths[ 0x0027 ] = 191;
            CodePointWidths[ 0x0072 ] = 333;
            CodePointWidths[ 0x00AE ] = 737;
            CodePointWidths[ 0x0073 ] = 500;
            CodePointWidths[ 0x009A ] = 500;
            CodePointWidths[ 0x00A7 ] = 556;
            CodePointWidths[ 0x003B ] = 278;
            CodePointWidths[ 0x0037 ] = 556;
            CodePointWidths[ 0x0036 ] = 556;
            CodePointWidths[ 0x002F ] = 278;
            CodePointWidths[ 0x0020 ] = 278;
            CodePointWidths[ 0x00A0 ] = 278;
            CodePointWidths[ 0x00A3 ] = 556;
            CodePointWidths[ 0x0074 ] = 278;
            CodePointWidths[ 0x00FE ] = 556;
            CodePointWidths[ 0x0033 ] = 556;
            CodePointWidths[ 0x00BE ] = 834;
            CodePointWidths[ 0x00B3 ] = 333;
            CodePointWidths[ 0x0098 ] = 333;
            CodePointWidths[ 0x0099 ] = 1000;
            CodePointWidths[ 0x0032 ] = 556;
            CodePointWidths[ 0x00B2 ] = 333;
            CodePointWidths[ 0x0075 ] = 556;
            CodePointWidths[ 0x00FA ] = 556;
            CodePointWidths[ 0x00FB ] = 556;
            CodePointWidths[ 0x00FC ] = 556;
            CodePointWidths[ 0x00F9 ] = 556;
            CodePointWidths[ 0x005F ] = 556;
            CodePointWidths[ 0x0076 ] = 500;
            CodePointWidths[ 0x0077 ] = 722;
            CodePointWidths[ 0x0078 ] = 500;
            CodePointWidths[ 0x0079 ] = 500;
            CodePointWidths[ 0x00FD ] = 500;
            CodePointWidths[ 0x00FF ] = 500;
            CodePointWidths[ 0x00A5 ] = 556;
            CodePointWidths[ 0x007A ] = 500;
            CodePointWidths[ 0x0030 ] = 556;
        }

        public Helvetica()
            : base( "Helvetica", "WinAnsiEncoding", 718, 718, -207, 32, 255, CodePointWidths, DefaultMapping )
        {
        }
    }
}