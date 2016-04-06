using System;
using System.Globalization;
using Fonet.Util;

namespace Fonet.DataTypes
{
    internal class ColorType : ICloneable
    {
        private static readonly string[] names =
        {
            "aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige",
            "bisque", "black", "blanchedalmond", "blue", "blueviolet", "brown",
            "burlywood", "cadetblue", "chartreuse", "chocolate", "coral",
            "cornflowerblue", "cornsilk", "crimson", "cyan", "darkblue",
            "darkcyan", "darkgoldenrod", "darkgray", "darkgreen", "darkgrey",
            "darkkhaki", "darkmagenta", "darkolivegreen", "darkorange",
            "darkorchid", "darkred", "darksalmon", "darkseagreen",
            "darkslateblue", "darkslategray", "darkslategrey", "darkturquoise",
            "darkviolet", "deeppink", "deepskyblue", "dimgray", "dimgrey",
            "dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia",
            "gainsboro", "lightpink", "lightsalmon", "lightseagreen",
            "lightskyblue", "lightslategray", "lightslategrey", "lightsteelblue",
            "lightyellow", "lime", "limegreen", "linen", "magenta", "maroon",
            "mediumaquamarine", "mediumblue", "mediumorchid", "mediumpurple",
            "mediumseagreen", "mediumslateblue", "mediumspringgreen",
            "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream",
            "mistyrose", "moccasin", "navajowhite", "navy", "oldlace", "olive",
            "olivedrab", "orange", "orangered", "orchid", "palegoldenrod",
            "palegreen", "paleturquoise", "palevioletred", "papayawhip",
            "peachpuff", "peru", "pink", "plum", "powderblue", "purple", "red",
            "rosybrown", "royalblue", "saddlebrown", "salmon", "ghostwhite",
            "gold", "goldenrod", "gray", "grey", "green", "greenyellow",
            "honeydew", "hotpink", "indianred", "indigo", "ivory", "khaki",
            "lavender", "lavenderblush", "lawngreen", "lemonchiffon",
            "lightblue", "lightcoral", "lightcyan", "lightgoldenrodyellow",
            "lightgray", "lightgreen", "lightgrey", "sandybrown", "seagreen",
            "seashell", "sienna", "silver", "skyblue", "slateblue", "slategray",
            "slategrey", "snow", "springgreen", "steelblue", "tan", "teal",
            "thistle", "tomato", "turquoise", "violet", "wheat", "white",
            "whitesmoke", "yellow", "yellowgreen"
        };

        private static readonly int[ , ] vals =
        {
            { 240, 248, 255 },
            { 250, 235, 215 },
            { 0, 255, 255 },
            { 127, 255, 212 },
            { 240, 255, 255 },
            { 245, 245, 220 },
            { 255, 228, 196 },
            { 0, 0, 0 },
            { 255, 235, 205 },
            { 0, 0, 255 },
            { 138, 43, 226 },
            { 165, 42, 42 },
            { 222, 184, 135 },
            { 95, 158, 160 },
            { 127, 255, 0 },
            { 210, 105, 30 },
            { 255, 127, 80 },
            { 100, 149, 237 },
            { 255, 248, 220 },
            { 220, 20, 60 },
            { 0, 255, 255 },
            { 0, 0, 139 },
            { 0, 139, 139 },
            { 184, 134, 11 },
            { 169, 169, 169 },
            { 0, 100, 0 },
            { 169, 169, 169 },
            { 189, 183, 107 },
            { 139, 0, 139 },
            { 85, 107, 47 },
            { 255, 140, 0 },
            { 153, 50, 204 },
            { 139, 0, 0 },
            { 233, 150, 122 },
            { 143, 188, 143 },
            { 72, 61, 139 },
            { 47, 79, 79 },
            { 47, 79, 79 },
            { 0, 206, 209 },
            { 148, 0, 211 },
            { 255, 20, 147 },
            { 0, 191, 255 },
            { 105, 105, 105 },
            { 105, 105, 105 },
            { 30, 144, 255 },
            { 178, 34, 34 },
            { 255, 250, 240 },
            { 34, 139, 34 },
            { 255, 0, 255 },
            { 220, 220, 220 },
            { 255, 182, 193 },
            { 255, 160, 122 },
            { 32, 178, 170 },
            { 135, 206, 250 },
            { 119, 136, 153 },
            { 119, 136, 153 },
            { 176, 196, 222 },
            { 255, 255, 224 },
            { 0, 255, 0 },
            { 50, 205, 50 },
            { 250, 240, 230 },
            { 255, 0, 255 },
            { 128, 0, 0 },
            { 102, 205, 170 },
            { 0, 0, 205 },
            { 186, 85, 211 },
            { 147, 112, 219 },
            { 60, 179, 113 },
            { 123, 104, 238 },
            { 0, 250, 154 },
            { 72, 209, 204 },
            { 199, 21, 133 },
            { 25, 25, 112 },
            { 245, 255, 250 },
            { 255, 228, 225 },
            { 255, 228, 181 },
            { 255, 222, 173 },
            { 0, 0, 128 },
            { 253, 245, 230 },
            { 128, 128, 0 },
            { 107, 142, 35 },
            { 255, 165, 0 },
            { 255, 69, 0 },
            { 218, 112, 214 },
            { 238, 232, 170 },
            { 152, 251, 152 },
            { 175, 238, 238 },
            { 219, 112, 147 },
            { 255, 239, 213 },
            { 255, 218, 185 },
            { 205, 133, 63 },
            { 255, 192, 203 },
            { 221, 160, 221 },
            { 176, 224, 230 },
            { 128, 0, 128 },
            { 255, 0, 0 },
            { 188, 143, 143 },
            { 65, 105, 225 },
            { 139, 69, 19 },
            { 250, 128, 114 },
            { 248, 248, 255 },
            { 255, 215, 0 },
            { 218, 165, 32 },
            { 128, 128, 128 },
            { 128, 128, 128 },
            { 0, 128, 0 },
            { 173, 255, 47 },
            { 240, 255, 240 },
            { 255, 105, 180 },
            { 205, 92, 92 },
            { 75, 0, 130 },
            { 255, 255, 240 },
            { 240, 230, 140 },
            { 230, 230, 250 },
            { 255, 240, 245 },
            { 124, 252, 0 },
            { 255, 250, 205 },
            { 173, 216, 230 },
            { 240, 128, 128 },
            { 224, 255, 255 },
            { 250, 250, 210 },
            { 211, 211, 211 },
            { 144, 238, 144 },
            { 211, 211, 211 },
            { 244, 164, 96 },
            { 46, 139, 87 },
            { 255, 245, 238 },
            { 160, 82, 45 },
            { 192, 192, 192 },
            { 135, 206, 235 },
            { 106, 90, 205 },
            { 112, 128, 144 },
            { 112, 128, 144 },
            { 255, 250, 250 },
            { 0, 255, 127 },
            { 70, 130, 180 },
            { 210, 180, 140 },
            { 0, 128, 128 },
            { 216, 191, 216 },
            { 255, 99, 71 },
            { 64, 224, 208 },
            { 238, 130, 238 },
            { 245, 222, 179 },
            { 255, 255, 255 },
            { 245, 245, 245 },
            { 255, 255, 0 },
            { 154, 205, 50 }
        };

        public ColorType( float red, float green, float blue )
        {
            Alpha = 0;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ColorType( string value )
        {
            Alpha = 0;
            string colorValue = value.ToLower();

            if ( colorValue.StartsWith( "#" ) )
            {
                try
                {
                    if ( colorValue.Length == 4 )
                    {
                        // note: divide by 15 so F = FF = 1 and so on
                        Red = int.Parse(
                            colorValue.Substring( 1, 1 ), NumberStyles.HexNumber ) / 15f;
                        Green = int.Parse(
                            colorValue.Substring( 2, 1 ), NumberStyles.HexNumber ) / 15f;
                        Blue = int.Parse(
                            colorValue.Substring( 3, 1 ), NumberStyles.HexNumber ) / 15f;
                    }
                    else if ( colorValue.Length == 7 )
                    {
                        // note: divide by 255 so FF = 1
                        Red = int.Parse(
                            colorValue.Substring( 1, 2 ), NumberStyles.HexNumber ) / 255f;
                        Green = int.Parse(
                            colorValue.Substring( 3, 2 ), NumberStyles.HexNumber ) / 255f;
                        Blue = int.Parse(
                            colorValue.Substring( 5, 2 ), NumberStyles.HexNumber ) / 255f;
                    }
                    else
                    {
                        Red = 0;
                        Green = 0;
                        Blue = 0;
                        FonetDriver.ActiveDriver.FireFonetError(
                            "Unknown colour format. Must be #RGB or #RRGGBB" );
                    }
                }
                catch ( Exception )
                {
                    Red = 0;
                    Green = 0;
                    Blue = 0;
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Unknown colour format. Must be #RGB or #RRGGBB" );
                }
            }
            else if ( colorValue.StartsWith( "rgb(" ) )
            {
                int poss = colorValue.IndexOf( "(" );
                int pose = colorValue.IndexOf( ")" );
                if ( poss != -1 && pose != -1 )
                {
                    colorValue = colorValue.Substring( poss + 1, pose );
                    var st = new StringTokenizer( colorValue, "," );
                    try
                    {
                        if ( st.HasMoreTokens() )
                        {
                            string str = st.NextToken().Trim();
                            if ( str.EndsWith( "%" ) )
                            {
                                Red =
                                    int.Parse( str.Substring( 0, str.Length - 1 ) )
                                        * 2.55f;
                            }
                            else
                                Red = int.Parse( str ) / 255f;
                        }
                        if ( st.HasMoreTokens() )
                        {
                            string str = st.NextToken().Trim();
                            if ( str.EndsWith( "%" ) )
                            {
                                Green =
                                    int.Parse( str.Substring( 0, str.Length - 1 ) )
                                        * 2.55f;
                            }
                            else
                                Green = int.Parse( str ) / 255f;
                        }
                        if ( st.HasMoreTokens() )
                        {
                            string str = st.NextToken().Trim();
                            if ( str.EndsWith( "%" ) )
                            {
                                Blue =
                                    int.Parse( str.Substring( 0, str.Length - 1 ) )
                                        * 2.55f;
                            }
                            else
                                Blue = int.Parse( str ) / 255f;
                        }
                    }
                    catch
                    {
                        Red = 0;
                        Green = 0;
                        Blue = 0;
                        FonetDriver.ActiveDriver.FireFonetError(
                            "Unknown colour format. Must be #RGB or #RRGGBB" );
                    }
                }
            }
            else if ( colorValue.StartsWith( "url(" ) )
            {
                // refers to a gradient
                FonetDriver.ActiveDriver.FireFonetError(
                    "unsupported color format" );
            }
            else
            {
                if ( colorValue.Equals( "transparent" ) )
                {
                    Red = 0;
                    Green = 0;
                    Blue = 0;
                    Alpha = 1;
                }
                else
                {
                    var found = false;
                    for ( var count = 0; count < names.Length; count++ )
                    {
                        if ( colorValue.Equals( names[ count ] ) )
                        {
                            Red = vals[ count, 0 ] / 255f;
                            Green = vals[ count, 1 ] / 255f;
                            Blue = vals[ count, 2 ] / 255f;
                            found = true;
                            break;
                        }
                    }
                    if ( !found )
                    {
                        Red = 0;
                        Green = 0;
                        Blue = 0;
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Unknown colour name: " + colorValue + ".  Defaulting to black." );
                    }
                }
            }
        }

        public float Blue { get; set; }

        public float Green { get; set; }

        public float Red { get; set; }

        public float Alpha { get; set; }

        public object Clone()
        {
            return new ColorType( Red, Green, Blue );
        }
    }
}