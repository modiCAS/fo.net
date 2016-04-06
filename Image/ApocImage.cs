using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Fonet.DataTypes;
using Fonet.Pdf.Filter;

namespace Fonet.Image
{
    /// <summary>
    ///     A bitmap image that will be referenced by fo:external-graphic.
    /// </summary>
    internal sealed unsafe class FonetImage
    {
        public const int DEFAULT_BITPLANES = 8;
        private BitmapData bitmapData;

        /// <summary>
        ///     Filter that will be applied to image data
        /// </summary>
        private IFilter filter;

        // image height

        // Image data 

        // Bits per pixel

        // Image color space 

        // Image URL
        private readonly string m_href;
        private byte* pBase = null;

        // Variables used by unsafe code
        private int scanWidth;

        // image width

        /// <summary>
        ///     Constructs a new FonetImage using the supplied bitmap.
        /// </summary>
        /// <remarks>
        ///     Does not hold a reference to the passed bitmap.  Instead the
        ///     image data is extracted from <b>bitmap</b> on construction.
        /// </remarks>
        /// <param name="href">The location of <i>bitmap</i></param>
        /// <param name="imageData">The image data</param>
        public FonetImage( string href, byte[] imageData )
        {
            m_href = href;

            ColorSpace = new ColorSpace( ColorSpace.DeviceRgb );
            BitsPerPixel = DEFAULT_BITPLANES; // 8

            // Bitmap does not seem to be thread-safe.  The only situation
            // Where this causes a problem is when the evaluation image is
            // used.  Each thread is given the same instance of Bitmap from
            // the resource manager.
            var bitmap = new Bitmap( new MemoryStream( imageData ) );

            Width = bitmap.Width;
            Height = bitmap.Height;
            Bitmaps = imageData;

            ExtractImage( bitmap );
        }

        /// <summary>
        ///     Return the image URL.
        /// </summary>
        /// <returns>the image URL (as a string)</returns>
        public string Uri
        {
            get { return m_href; }
        }

        /// <summary>
        ///     Return the image width.
        /// </summary>
        /// <returns>the image width</returns>
        public int Width { get; private set; }

        /// <summary>
        ///     Return the image height.
        /// </summary>
        /// <returns>the image height</returns>
        public int Height { get; private set; }

        /// <summary>
        ///     Return the number of bits per pixel.
        /// </summary>
        /// <returns>number of bits per pixel</returns>
        public int BitsPerPixel { get; private set; }

        /// <summary>
        ///     Return the image data size
        /// </summary>
        /// <returns>The image data size</returns>
        public int BitmapsSize
        {
            get { return Bitmaps != null ? Bitmaps.Length : 0; }
        }

        /// <summary>
        ///     Return the image data (uncompressed).
        /// </summary>
        /// <returns>the image data</returns>
        public byte[] Bitmaps { get; private set; }

        /// <summary>
        ///     Return the image color space.
        /// </summary>
        /// <returns>the image color space (Fonet.Datatypes.ColorSpace)</returns>
        public ColorSpace ColorSpace { get; private set; }

        /// <summary>
        ///     Returns the filter that should be applied to the bitmap data.
        /// </summary>
        public IFilter Filter
        {
            get { return filter; }
        }

        private Point GetPixelSize( Bitmap bitmap )
        {
            var unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds( ref unit );

            return new Point( (int)bounds.Width, (int)bounds.Height );
        }

        /// <summary>
        ///     Extracts the raw data from the image into a byte array suitable
        ///     for including in the PDF document.  The image is always extracted
        ///     as a 24-bit RGB image, regardless of it's original colour space
        ///     and colour depth.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap" /> from which the data is extracted</param>
        /// <returns>A byte array containing the raw 24-bit RGB data</returns>
        private void ExtractImage( Bitmap bitmap )
        {
            // This should be a factory when we handle more image types
            if ( bitmap.RawFormat.Equals( ImageFormat.Jpeg ) )
            {
                var parser = new JpegParser( Bitmaps );
                JpegInfo info = parser.Parse();

                BitsPerPixel = info.BitsPerSample;
                ColorSpace = new ColorSpace( info.ColourSpace );
                Width = info.Width;
                Height = info.Height;

                // A "no-op" filter since the JPEG data is already compressed
                filter = new DctFilter();
            }
            else
            {
                ExtractOtherImageBits( bitmap );

                // Performs zip compression
                filter = new FlateFilter();
            }
        }

        private void ExtractOtherImageBits( Bitmap bitmap )
        {
            // Get dimensions of bitmap in pixels
            Point size = GetPixelSize( bitmap );

            // 'Locks' bitmap bits in memory
            LockBitmap( bitmap );

            // The size of the required byte array is not only a factor of the 
            // width and height, but also the color components of each pixel. 
            // Each pixel requires three bytes of storage - one byte each for 
            // the red, green and blue components
            Bitmaps = new byte[ size.X * size.Y * 3 ];

            try
            {
                for ( var y = 0; y < size.Y; y++ )
                {
                    PixelData* pPixel = PixelAt( 0, y );
                    for ( var x = 0; x < size.X; x++ )
                    {
                        Bitmaps[ 3 * ( y * Width + x ) ] = pPixel->red;
                        Bitmaps[ 3 * ( y * Width + x ) + 1 ] = pPixel->green;
                        Bitmaps[ 3 * ( y * Width + x ) + 2 ] = pPixel->blue;
                        pPixel++;
                    }
                }
            }
            catch ( Exception e )
            {
                FonetDriver.ActiveDriver.FireFonetError( e.ToString() );
            }
            finally
            {
                // Should always unlock the bitmap from memory
                UnlockBitmap( bitmap );
            }
        }

        private void LockBitmap( Bitmap bitmap )
        {
            var unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds( ref unit );
            var bounds = new Rectangle( (int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height );

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            scanWidth = (int)boundsF.Width * sizeof( PixelData );
            if ( scanWidth % 4 != 0 )
                scanWidth = 4 * ( scanWidth / 4 + 1 );

            bitmapData =
                bitmap.LockBits( bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            pBase = (byte*)bitmapData.Scan0.ToPointer();
        }

        private PixelData* PixelAt( int x, int y )
        {
            return (PixelData*)( pBase + y * scanWidth + x * sizeof( PixelData ) );
        }

        private void UnlockBitmap( Bitmap bitmap )
        {
            bitmap.UnlockBits( bitmapData );
            bitmapData = null;
            pBase = null;
        }
    }

    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
    }
}