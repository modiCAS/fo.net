using System;
using System.Globalization;
using System.Text;
using Fonet.DataTypes;

namespace Fonet.Render.Pdf
{
    internal sealed class PdfColor
    {
        private readonly double _blue;
        private readonly double _green;
        private readonly double _red;

        public PdfColor( ColorType color )
        {
            _red = color.Red;
            _green = color.Green;
            _blue = color.Blue;
        }

        public PdfColor( double red, double green, double blue )
        {
            _red = red;
            _green = green;
            _blue = blue;
        }

        // components from 0 to 255
        public PdfColor( int red, int green, int blue )
            : this( red / 255d, green / 255d, blue / 255d )
        {
        }

        public double GetRed()
        {
            return _red;
        }

        public double GetGreen()
        {
            return _green;
        }

        public double GetBlue()
        {
            return _blue;
        }

        public string GetColorSpaceOut( bool fillNotStroke )
        {
            var p = new StringBuilder();

            // according to pdfspec 12.1 p.399
            // if the colors are the same then just use the g or G operator
            bool same = Math.Abs( _red - _green ) < 0.01f && Math.Abs( _red - _blue ) < 0.01f;

            // output RGB
            if ( fillNotStroke )
            {
                if ( same )
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} g\n",
                        _red );
                }
                else
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} rg\n",
                        _red, _green, _blue );
                }
            }
            else
            {
                if ( same )
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} G\n",
                        _red );
                }
                else
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} RG\n",
                        _red, _green, _blue );
                }
            }

            return p.ToString();
        }
    }
}