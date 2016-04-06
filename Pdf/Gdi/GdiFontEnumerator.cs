using System;
using System.Collections;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     Summary description for GdiFontEnumerator.
    /// </summary>
    public class GdiFontEnumerator
    {
        private const int RasterFont = 0x001;
        private const int DeviceFont = 0x002;
        private const int TrueTypeFont = 0x004;

        private const int ExtractFamilies = 1;
        private const int ExtractStyles = 2;

        private const byte AnsiCharset = 0;
        private const byte DefaultCharset = 1;
        private const byte SymbolCharset = 2;
        private readonly GdiDeviceContent dc;

        private readonly SortedList families = new SortedList();
        private readonly FontStyles styles = new FontStyles();

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="dc">A non-null reference to a wrapper around a GDI device context.</param>
        public GdiFontEnumerator( GdiDeviceContent dc )
        {
            this.dc = dc;
        }

        /// <summary>
        ///     Returns a list of font family names sorted in ascending order.
        /// </summary>
        public string[] FamilyNames
        {
            get
            {
                var lf = new LogFont();
                lf.lfCharSet = DefaultCharset;

                FontEnumDelegate font = EnumFontMethod;
                LibWrapper.EnumFontFamiliesEx( dc.Handle, lf, font, ExtractFamilies, 0 );

                return (string[])new ArrayList( families.Keys ).ToArray( typeof( string ) );
            }
        }

        /// <summary>
        ///     Returns a list of font styles associated with <i>familyName</i>.
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public FontStyles GetStyles( string familyName )
        {
            styles.Clear();
            FontEnumDelegate font = EnumFontMethod;
            LibWrapper.EnumFontFamilies( dc.Handle, familyName, font, ExtractStyles );

            return styles;
        }

        private int EnumFontMethod(
            ref EnumLogFont logFont,
            ref NewTextMetric textMetric,
            uint fontType,
            int lParam )
        {
            // Only interested in TrueType fonts
            if ( ( fontType & TrueTypeFont ) > 0 )
            {
                if ( lParam == ExtractFamilies )
                {
                    string familyName = logFont.elfLogFont.lfFaceName;
                    if ( !families.ContainsKey( familyName ) )
                        families.Add( familyName, string.Empty );
                }
                else if ( lParam == ExtractStyles )
                {
                    string styleName = new string( logFont.elfStyle ).Trim( '\0' );
                    if ( !styles.Contains( styleName ) )
                        styles.AddStyle( styleName );
                }
                else
                    throw new InvalidOperationException( "Unknown EnumFontMethod parameter." );
            }

            return 1;
        }
    }

    public class FontStyles
    {
        private readonly IDictionary styles = new Hashtable();

        public bool RegularAvailable
        {
            get { return styles.Contains( "Regular" ) || styles.Contains( "Normal" ); }
        }

        public bool BoldAvailable
        {
            get { return styles.Contains( "Bold" ); }
        }

        public bool ItalicAvailable
        {
            get { return styles.Contains( "Italic" ); }
        }

        public bool BoldItalicAvailable
        {
            get { return styles.Contains( "Bold Italic" ); }
        }

        internal void AddStyle( string styleName )
        {
            styles.Add( styleName, string.Empty );
        }

        internal void Clear()
        {
            styles.Clear();
        }

        internal bool Contains( string styleName )
        {
            return styles.Contains( styleName );
        }
    }
}