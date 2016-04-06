using Fonet.Pdf.Gdi;
using Fonet.Render.Pdf;
using Fonet.Render.Pdf.Fonts;

namespace Fonet.Layout
{
    internal class FontState
    {
        private readonly IFontMetric metric;

        public FontState( FontInfo fontInfo, string fontFamily, string fontStyle,
            string fontWeight, int fontSize, int fontVariant )
        {
            FontInfo = fontInfo;
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontWeight = fontWeight;
            FontSize = fontSize;
            FontName = fontInfo.FontLookup( fontFamily, fontStyle, fontWeight );
            metric = fontInfo.GetMetricsFor( FontName );
            FontVariant = fontVariant;
            LetterSpacing = 0;
        }

        public FontState( FontInfo fontInfo, string fontFamily, string fontStyle,
            string fontWeight, int fontSize, int fontVariant, int letterSpacing )
            : this( fontInfo, fontFamily, fontStyle, fontWeight, fontSize, fontVariant )
        {
            LetterSpacing = letterSpacing;
        }

        public int Ascender
        {
            get { return metric.Ascender * FontSize / 1000; }
        }

        public int LetterSpacing { get; private set; }

        public int CapHeight
        {
            get { return metric.CapHeight * FontSize / 1000; }
        }

        public int Descender
        {
            get { return metric.Descender * FontSize / 1000; }
        }

        public string FontName { get; private set; }

        public int FontSize { get; private set; }

        public string FontWeight { get; private set; }

        public string FontFamily { get; private set; }

        public string FontStyle { get; private set; }

        public int FontVariant { get; private set; }

        public FontInfo FontInfo { get; private set; }

        public GdiKerningPairs Kerning
        {
            get
            {
                IFontDescriptor descriptor = metric.Descriptor;
                if ( descriptor != null )
                {
                    if ( descriptor.HasKerningInfo )
                        return descriptor.KerningInfo;
                }
                return GdiKerningPairs.Empty;
            }
        }

        public int GetWidth( ushort charId )
        {
            return LetterSpacing + metric.GetWidth( charId ) * FontSize / 1000;
        }

        public ushort MapCharacter( char c )
        {
            if ( metric is Font )
                return ( (Font)metric ).MapCharacter( c );

            ushort charIndex = CodePointMapping.GetMapping( "WinAnsiEncoding" ).MapCharacter( c );
            if ( charIndex != 0 )
                return charIndex;
            return '#';
        }
    }
}