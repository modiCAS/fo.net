using System.Collections;

namespace Fonet.Fo.Properties
{
    internal class GenericColor : ColorTypeProperty.Maker
    {
        private static readonly Hashtable SHtKeywords;

        static GenericColor()
        {
            SHtKeywords = new Hashtable( 147 );
            SHtKeywords.Add( "aliceblue", "#f0f8ff" );
            SHtKeywords.Add( "antiquewhite", "#faebd7" );
            SHtKeywords.Add( "aqua", "#00ffff" );
            SHtKeywords.Add( "aquamarine", "#7fffd4" );
            SHtKeywords.Add( "azure", "#f0ffff" );
            SHtKeywords.Add( "beige", "#f5f5dc" );
            SHtKeywords.Add( "bisque", "#ffe4c4" );
            SHtKeywords.Add( "black", "#000000" );
            SHtKeywords.Add( "blanchedalmond", "#ffebcd" );
            SHtKeywords.Add( "blue", "#0000ff" );
            SHtKeywords.Add( "blueviolet", "#8a2be2" );
            SHtKeywords.Add( "brown", "#a52a2a" );
            SHtKeywords.Add( "burlywood", "#deb887" );
            SHtKeywords.Add( "cadetblue", "#5f9ea0" );
            SHtKeywords.Add( "chartreuse", "#7fff00" );
            SHtKeywords.Add( "chocolate", "#d2691e" );
            SHtKeywords.Add( "coral", "#ff7f50" );
            SHtKeywords.Add( "cornflowerblue", "#6495ed" );
            SHtKeywords.Add( "cornsilk", "#fff8dc" );
            SHtKeywords.Add( "crimson", "#dc143c" );
            SHtKeywords.Add( "cyan", "#00ffff" );
            SHtKeywords.Add( "darkblue", "#00008b" );
            SHtKeywords.Add( "darkcyan", "#008b8b" );
            SHtKeywords.Add( "darkgoldenrod", "#b8860b" );
            SHtKeywords.Add( "darkgray", "#a9a9a9" );
            SHtKeywords.Add( "darkgreen", "#006400" );
            SHtKeywords.Add( "darkgrey", "#a9a9a9" );
            SHtKeywords.Add( "darkkhaki", "#bdb76b" );
            SHtKeywords.Add( "darkmagenta", "#8b008b" );
            SHtKeywords.Add( "darkolivegreen", "#556b2f" );
            SHtKeywords.Add( "darkorange", "#ff8c00" );
            SHtKeywords.Add( "darkorchid", "#9932cc" );
            SHtKeywords.Add( "darkred", "#8b0000" );
            SHtKeywords.Add( "darksalmon", "#e9967a" );
            SHtKeywords.Add( "darkseagreen", "#8fbc8f" );
            SHtKeywords.Add( "darkslateblue", "#483d8b" );
            SHtKeywords.Add( "darkslategray", "#2f4f4f" );
            SHtKeywords.Add( "darkslategrey", "#2f4f4f" );
            SHtKeywords.Add( "darkturquoise", "#00ced1" );
            SHtKeywords.Add( "darkviolet", "#9400d3" );
            SHtKeywords.Add( "deeppink", "#ff1493" );
            SHtKeywords.Add( "deepskyblue", "#00bfff" );
            SHtKeywords.Add( "dimgray", "#696969" );
            SHtKeywords.Add( "dimgrey", "#696969" );
            SHtKeywords.Add( "dodgerblue", "#1e90ff" );
            SHtKeywords.Add( "firebrick", "#b22222" );
            SHtKeywords.Add( "floralwhite", "#fffaf0" );
            SHtKeywords.Add( "forestgreen", "#228b22" );
            SHtKeywords.Add( "fuchsia", "#ff00ff" );
            SHtKeywords.Add( "gainsboro", "#dcdcdc" );
            SHtKeywords.Add( "lightpink", "#ffb6c1" );
            SHtKeywords.Add( "lightsalmon", "#ffa07a" );
            SHtKeywords.Add( "lightseagreen", "#20b2aa" );
            SHtKeywords.Add( "lightskyblue", "#87cefa" );
            SHtKeywords.Add( "lightslategray", "#778899" );
            SHtKeywords.Add( "lightslategrey", "#778899" );
            SHtKeywords.Add( "lightsteelblue", "#b0c4de" );
            SHtKeywords.Add( "lightyellow", "#ffffe0" );
            SHtKeywords.Add( "lime", "#00ff00" );
            SHtKeywords.Add( "limegreen", "#32cd32" );
            SHtKeywords.Add( "linen", "#faf0e6" );
            SHtKeywords.Add( "magenta", "#ff00ff" );
            SHtKeywords.Add( "maroon", "#800000" );
            SHtKeywords.Add( "mediumaquamarine", "#66cdaa" );
            SHtKeywords.Add( "mediumblue", "#0000cd" );
            SHtKeywords.Add( "mediumorchid", "#ba55d3" );
            SHtKeywords.Add( "mediumpurple", "#9370db" );
            SHtKeywords.Add( "mediumseagreen", "#3cb371" );
            SHtKeywords.Add( "mediumslateblue", "#7b68ee" );
            SHtKeywords.Add( "mediumspringgreen", "#00fa9a" );
            SHtKeywords.Add( "mediumturquoise", "#48d1cc" );
            SHtKeywords.Add( "mediumvioletred", "#c71585" );
            SHtKeywords.Add( "midnightblue", "#191970" );
            SHtKeywords.Add( "mintcream", "#f5fffa" );
            SHtKeywords.Add( "mistyrose", "#ffe4e1" );
            SHtKeywords.Add( "moccasin", "#ffe4b5" );
            SHtKeywords.Add( "navajowhite", "#ffdead" );
            SHtKeywords.Add( "navy", "#000080" );
            SHtKeywords.Add( "oldlace", "#fdf5e6" );
            SHtKeywords.Add( "olive", "#808000" );
            SHtKeywords.Add( "olivedrab", "#6b8e23" );
            SHtKeywords.Add( "orange", "#ffa500" );
            SHtKeywords.Add( "orangered", "#ff4500" );
            SHtKeywords.Add( "orchid", "#da70d6" );
            SHtKeywords.Add( "palegoldenrod", "#eee8aa" );
            SHtKeywords.Add( "palegreen", "#98fb98" );
            SHtKeywords.Add( "paleturquoise", "#afeeee" );
            SHtKeywords.Add( "palevioletred", "#db7093" );
            SHtKeywords.Add( "papayawhip", "#ffefd5" );
            SHtKeywords.Add( "peachpuff", "#ffdab9" );
            SHtKeywords.Add( "peru", "#cd853f" );
            SHtKeywords.Add( "pink", "#ffc0cb" );
            SHtKeywords.Add( "plum", "#dda0dd" );
            SHtKeywords.Add( "powderblue", "#b0e0e6" );
            SHtKeywords.Add( "purple", "#800080" );
            SHtKeywords.Add( "red", "#ff0000" );
            SHtKeywords.Add( "rosybrown", "#bc8f8f" );
            SHtKeywords.Add( "royalblue", "#4169e1" );
            SHtKeywords.Add( "saddlebrown", "#8b4513" );
            SHtKeywords.Add( "salmon", "#fa8072" );
            SHtKeywords.Add( "ghostwhite", "#f8f8ff" );
            SHtKeywords.Add( "gold", "#ffd700" );
            SHtKeywords.Add( "goldenrod", "#daa520" );
            SHtKeywords.Add( "gray", "#808080" );
            SHtKeywords.Add( "grey", "#808080" );
            SHtKeywords.Add( "green", "#008000" );
            SHtKeywords.Add( "greenyellow", "#adff2f" );
            SHtKeywords.Add( "honeydew", "#f0fff0" );
            SHtKeywords.Add( "hotpink", "#ff69b4" );
            SHtKeywords.Add( "indianred", "#cd5c5c" );
            SHtKeywords.Add( "indigo", "#4b0082" );
            SHtKeywords.Add( "ivory", "#fffff0" );
            SHtKeywords.Add( "khaki", "#f0e68c" );
            SHtKeywords.Add( "lavender", "#e6e6fa" );
            SHtKeywords.Add( "lavenderblush", "#fff0f5" );
            SHtKeywords.Add( "lawngreen", "#7cfc00" );
            SHtKeywords.Add( "lemonchiffon", "#fffacd" );
            SHtKeywords.Add( "lightblue", "#add8e6" );
            SHtKeywords.Add( "lightcoral", "#f08080" );
            SHtKeywords.Add( "lightcyan", "#e0ffff" );
            SHtKeywords.Add( "lightgoldenrodyellow", "#fafad2" );
            SHtKeywords.Add( "lightgray", "#d3d3d3" );
            SHtKeywords.Add( "lightgreen", "#90ee90" );
            SHtKeywords.Add( "lightgrey", "#d3d3d3" );
            SHtKeywords.Add( "sandybrown", "#f4a460" );
            SHtKeywords.Add( "seagreen", "#2e8b57" );
            SHtKeywords.Add( "seashell", "#fff5ee" );
            SHtKeywords.Add( "sienna", "#a0522d" );
            SHtKeywords.Add( "silver", "#c0c0c0" );
            SHtKeywords.Add( "skyblue", "#87ceeb" );
            SHtKeywords.Add( "slateblue", "#6a5acd" );
            SHtKeywords.Add( "slategray", "#708090" );
            SHtKeywords.Add( "slategrey", "#708090" );
            SHtKeywords.Add( "snow", "#fffafa" );
            SHtKeywords.Add( "springgreen", "#00ff7f" );
            SHtKeywords.Add( "steelblue", "#4682b4" );
            SHtKeywords.Add( "tan", "#d2b48c" );
            SHtKeywords.Add( "teal", "#008080" );
            SHtKeywords.Add( "thistle", "#d8bfd8" );
            SHtKeywords.Add( "tomato", "#ff6347" );
            SHtKeywords.Add( "turquoise", "#40e0d0" );
            SHtKeywords.Add( "violet", "#ee82ee" );
            SHtKeywords.Add( "wheat", "#f5deb3" );
            SHtKeywords.Add( "white", "#ffffff" );
            SHtKeywords.Add( "whitesmoke", "#f5f5f5" );
            SHtKeywords.Add( "yellow", "#ffff00" );
            SHtKeywords.Add( "yellowgreen", "#9acd32" );
        }

        protected GenericColor( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GenericColor( propName );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            var val = (string)SHtKeywords[ keyword ];
            if ( val == null )
                return base.CheckValueKeywords( keyword );
            return val;
        }
    }
}