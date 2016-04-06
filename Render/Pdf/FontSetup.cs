using System.Collections;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;
using Fonet.Render.Pdf.Fonts;

namespace Fonet.Render.Pdf
{
    /// <summary>
    ///     Sets up the PDF fonts.
    /// </summary>
    /// <remarks>
    ///     Assigns the font (with metrics) to internal names like "F1" and
    ///     assigns family-style-weight triplets to the fonts.
    /// </remarks>
    internal class FontSetup
    {
        /// <summary>
        ///     Handles mapping font triplets to a IFontMetric implementor
        /// </summary>
        private readonly FontInfo _fontInfo;

        /// <summary>
        ///     First 16 indices are used by base 14 and generic fonts
        /// </summary>
        private int _startIndex = 17;

        public FontSetup( FontInfo fontInfo, FontType proxyFontType )
        {
            this._fontInfo = fontInfo;

            // Add the base 14 fonts
            AddBase14Fonts();
            AddSystemFonts( proxyFontType );
        }

        /// <summary>
        ///     Adds all the system fonts to the FontInfo object.
        /// </summary>
        /// <remarks>
        ///     Adds metrics for basic fonts and useful family-style-weight
        ///     triplets for lookup.
        /// </remarks>
        /// <param name="fontType">Determines what type of font to instantiate.</param>
        private void AddSystemFonts( FontType fontType )
        {
            var enumerator = new GdiFontEnumerator( new GdiDeviceContent() );
            foreach ( string familyName in enumerator.FamilyNames )
            {
                if ( IsBase14FontName( familyName ) )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Will ignore TrueType font '" + familyName +
                            "' because a base 14 font with the same name already exists." );
                }
                else
                {
                    FontStyles styles = enumerator.GetStyles( familyName );

                    string name = GetNextAvailableName();
                    _fontInfo.AddMetrics( name, new ProxyFont( new FontProperties( familyName, false, false ), fontType ) );
                    _fontInfo.AddFontProperties( name, familyName, "normal", "normal" );

                    name = GetNextAvailableName();
                    _fontInfo.AddMetrics( name, new ProxyFont( new FontProperties( familyName, true, false ), fontType ) );
                    _fontInfo.AddFontProperties( name, familyName, "normal", "bold" );

                    name = GetNextAvailableName();
                    _fontInfo.AddMetrics( name, new ProxyFont( new FontProperties( familyName, false, true ), fontType ) );
                    _fontInfo.AddFontProperties( name, familyName, "italic", "normal" );

                    name = GetNextAvailableName();
                    _fontInfo.AddMetrics( name, new ProxyFont( new FontProperties( familyName, true, true ), fontType ) );
                    _fontInfo.AddFontProperties( name, familyName, "italic", "bold" );
                }
            }

            // Cursive - Monotype Corsiva
            _fontInfo.AddMetrics( "F15",
                new ProxyFont( new FontProperties( "Monotype Corsiva", false, false ), fontType ) );
            _fontInfo.AddFontProperties( "F15", "cursive", "normal", "normal" );

            // Fantasy - Zapf Dingbats
            _fontInfo.AddMetrics( "F16", Base14Font.ZapfDingbats );
            _fontInfo.AddFontProperties( "F16", "fantasy", "normal", "normal" );
        }

        /// <summary>
        ///     Returns <b>true</b> is <i>familyName</i> represents one of the
        ///     base 14 fonts; otherwise <b>false</b>.
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        private bool IsBase14FontName( string familyName )
        {
            switch ( familyName )
            {
            case "any":
            case "sans-serif":
            case "serif":
            case "monospace":
            case "Helvetica":
            case "Times":
            case "Courier":
            case "Symbol":
            case "ZapfDingbats":
                return true;
            default:
                return false;
            }
        }

        /// <summary>
        ///     Gets the next available font name.  A font name is defined as an
        ///     integer prefixed by the letter 'F'.
        /// </summary>
        /// <returns></returns>
        private string GetNextAvailableName()
        {
            return string.Format( "F{0}", _startIndex++ );
        }

        private void AddBase14Fonts()
        {
            _fontInfo.AddMetrics( "F1", Base14Font.Helvetica );
            _fontInfo.AddMetrics( "F2", Base14Font.HelveticaItalic );
            _fontInfo.AddMetrics( "F3", Base14Font.HelveticaBold );
            _fontInfo.AddMetrics( "F4", Base14Font.HelveticaBoldItalic );
            _fontInfo.AddMetrics( "F5", Base14Font.Times );
            _fontInfo.AddMetrics( "F6", Base14Font.TimesItalic );
            _fontInfo.AddMetrics( "F7", Base14Font.TimesBold );
            _fontInfo.AddMetrics( "F8", Base14Font.TimesBoldItalic );
            _fontInfo.AddMetrics( "F9", Base14Font.Courier );
            _fontInfo.AddMetrics( "F10", Base14Font.CourierItalic );
            _fontInfo.AddMetrics( "F11", Base14Font.CourierBold );
            _fontInfo.AddMetrics( "F12", Base14Font.CourierBoldItalic );
            _fontInfo.AddMetrics( "F13", Base14Font.Symbol );
            _fontInfo.AddMetrics( "F14", Base14Font.ZapfDingbats );

            _fontInfo.AddFontProperties( "F5", "any", "normal", "normal" );
            _fontInfo.AddFontProperties( "F6", "any", "italic", "normal" );
            _fontInfo.AddFontProperties( "F6", "any", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F7", "any", "normal", "bold" );
            _fontInfo.AddFontProperties( "F8", "any", "italic", "bold" );
            _fontInfo.AddFontProperties( "F8", "any", "oblique", "bold" );

            _fontInfo.AddFontProperties( "F1", "sans-serif", "normal", "normal" );
            _fontInfo.AddFontProperties( "F2", "sans-serif", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F2", "sans-serif", "italic", "normal" );
            _fontInfo.AddFontProperties( "F3", "sans-serif", "normal", "bold" );
            _fontInfo.AddFontProperties( "F4", "sans-serif", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F4", "sans-serif", "italic", "bold" );
            _fontInfo.AddFontProperties( "F5", "serif", "normal", "normal" );
            _fontInfo.AddFontProperties( "F6", "serif", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F6", "serif", "italic", "normal" );
            _fontInfo.AddFontProperties( "F7", "serif", "normal", "bold" );
            _fontInfo.AddFontProperties( "F8", "serif", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F8", "serif", "italic", "bold" );
            _fontInfo.AddFontProperties( "F9", "monospace", "normal", "normal" );
            _fontInfo.AddFontProperties( "F10", "monospace", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F10", "monospace", "italic", "normal" );
            _fontInfo.AddFontProperties( "F11", "monospace", "normal", "bold" );
            _fontInfo.AddFontProperties( "F12", "monospace", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F12", "monospace", "italic", "bold" );

            _fontInfo.AddFontProperties( "F1", "Helvetica", "normal", "normal" );
            _fontInfo.AddFontProperties( "F2", "Helvetica", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F2", "Helvetica", "italic", "normal" );
            _fontInfo.AddFontProperties( "F3", "Helvetica", "normal", "bold" );
            _fontInfo.AddFontProperties( "F4", "Helvetica", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F4", "Helvetica", "italic", "bold" );
            _fontInfo.AddFontProperties( "F5", "Times", "normal", "normal" );
            _fontInfo.AddFontProperties( "F6", "Times", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F6", "Times", "italic", "normal" );
            _fontInfo.AddFontProperties( "F7", "Times", "normal", "bold" );
            _fontInfo.AddFontProperties( "F8", "Times", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F8", "Times", "italic", "bold" );
            _fontInfo.AddFontProperties( "F9", "Courier", "normal", "normal" );
            _fontInfo.AddFontProperties( "F10", "Courier", "oblique", "normal" );
            _fontInfo.AddFontProperties( "F10", "Courier", "italic", "normal" );
            _fontInfo.AddFontProperties( "F11", "Courier", "normal", "bold" );
            _fontInfo.AddFontProperties( "F12", "Courier", "oblique", "bold" );
            _fontInfo.AddFontProperties( "F12", "Courier", "italic", "bold" );
            _fontInfo.AddFontProperties( "F13", "Symbol", "normal", "normal" );
            _fontInfo.AddFontProperties( "F14", "ZapfDingbats", "normal", "normal" );
        }

        /// <summary>
        ///     Add the fonts in the font info to the PDF document.
        /// </summary>
        /// <param name="fontCreator">Object that creates PdfFont objects.</param>
        /// <param name="resources">Resources object to add fonts too.</param>
        internal void AddToResources( PdfFontCreator fontCreator, PdfResources resources )
        {
            Hashtable fonts = _fontInfo.GetUsedFonts();
            foreach ( string fontName in fonts.Keys )
            {
                var font = (Font)fonts[ fontName ];
                resources.AddFont( fontCreator.MakeFont( fontName, font ) );
            }
        }
    }
}