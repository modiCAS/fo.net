using System;
using System.Collections;

namespace Fonet.Layout
{
    internal class FontInfo
    {
        private readonly Hashtable _fonts;
        private readonly Hashtable _triplets;
        private readonly Hashtable _usedFonts;

        public FontInfo()
        {
            _triplets = new Hashtable();
            _fonts = new Hashtable();
            _usedFonts = new Hashtable();
        }

        public void AddFontProperties( string name, string family, string style, string weight )
        {
            string key = CreateFontKey( family, style, weight );
            _triplets.Add( key, name );
        }

        public void AddMetrics( string name, IFontMetric metrics )
        {
            _fonts.Add( name, metrics );
        }

        public string FontLookup( string family, string style, string weight )
        {
            return FontLookup( CreateFontKey( family, style, weight ) );
        }

        private string FontLookup( string key )
        {
            var f = (string)_triplets[ key ];
            if ( f == null )
            {
                int i = key.IndexOf( ',' );
                string s = "any" + key.Substring( i );
                f = (string)_triplets[ s ];
                if ( f == null )
                {
                    f = (string)_triplets[ "any,normal,normal" ];
                    if ( f == null )
                        throw new FonetException( "no default font defined by OutputConverter" );
                    FonetDriver.ActiveDriver.FireFonetInfo(
                        "Defaulted font to any,normal,normal" );
                }
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Unknown font " + key + " so defaulted font to any" );
            }

            _usedFonts[ f ] = _fonts[ f ];
            return f;
        }

        private bool HasFont( string family, string style, string weight )
        {
            string key = CreateFontKey( family, style, weight );
            return _triplets.ContainsKey( key );
        }

        public static string CreateFontKey( string family, string style, string weight )
        {
            int i;
            try
            {
                if ( !string.IsNullOrEmpty( weight ) && char.IsNumber( weight, 0 ) )
                    i = int.Parse( weight );
                else
                    i = 0;
            }
            catch ( Exception )
            {
                i = 0;
            }

            if ( i > 600 )
                weight = "bold";
            else if ( i > 0 )
                weight = "normal";

            return string.Format( "{0},{1},{2}", family, style, weight );
        }

        public IDictionary GetFonts()
        {
            return _fonts;
        }

        public IFontMetric GetFontByName( string fontName )
        {
            return (IFontMetric)_fonts[ fontName ];
        }

        public Hashtable GetUsedFonts()
        {
            return _usedFonts;
        }

        public IFontMetric GetMetricsFor( string fontName )
        {
            _usedFonts[ fontName ] = _fonts[ fontName ];
            return (IFontMetric)_fonts[ fontName ];
        }
    }
}