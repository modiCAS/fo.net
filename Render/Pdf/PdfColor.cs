using System.Globalization;
using System.Text;
using Fonet.DataTypes;

namespace Fonet.Render.Pdf
{
    internal sealed class PdfColor
    {
        private readonly double blue = -1.0;
        private readonly double green = -1.0;
        private readonly double red = -1.0;

        public PdfColor( ColorType color )
        {
            red = color.Red;
            green = color.Green;
            blue = color.Blue;
        }

        public PdfColor( double red, double green, double blue )
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        // components from 0 to 255
        public PdfColor( int red, int green, int blue ) : this(
            red / 255d,
            green / 255d,
            blue / 255d
            )
        {
        }

        public double getRed()
        {
            return red;
        }

        public double getGreen()
        {
            return green;
        }

        public double getBlue()
        {
            return blue;
        }

        public string getColorSpaceOut( bool fillNotStroke )
        {
            var p = new StringBuilder();

            // according to pdfspec 12.1 p.399
            // if the colors are the same then just use the g or G operator
            var same = false;
            if ( red == green && red == blue )
                same = true;

            // output RGB
            if ( fillNotStroke )
            {
                if ( same )
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} g\n",
                        red );
                }
                else
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} rg\n",
                        red, green, blue );
                }
            }
            else
            {
                if ( same )
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} G\n",
                        red );
                }
                else
                {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} RG\n",
                        red, green, blue );
                }
            }

            return p.ToString();
        }
    }
}