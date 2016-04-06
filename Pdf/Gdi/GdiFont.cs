using System;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     A thin wrapper around a handle to a font
    /// </summary>
    public class GdiFont
    {
        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="hFont">A handle to an existing font.</param>
        /// <param name="faceName"></param>
        /// <param name="height"></param>
        public GdiFont( IntPtr hFont, string faceName, int height )
        {
            Handle = hFont;
            FaceName = faceName;
            Height = height;
        }

        public string FaceName { get; private set; }

        public int Height { get; private set; }

        public IntPtr Handle { get; private set; }

        /// <summary>
        ///     Class destructor
        /// </summary>
        ~GdiFont()
        {
            Dispose( false );
        }

        public virtual void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( Handle != IntPtr.Zero )
                {
                    //Console.WriteLine("Dispoing of font {0}, {1}pt ({2})", faceName, height, hFont);
                    LibWrapper.DeleteObject( Handle );
                    Handle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///     Creates a font based on the supplied typeface name and size.
        /// </summary>
        /// <param name="faceName">The typeface name of a font.</param>
        /// <param name="height">
        ///     The height, in logical units, of the font's character
        ///     cell or character.
        /// </param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <returns></returns>
        public static GdiFont CreateFont( string faceName, int height, bool bold, bool italic )
        {
            var lf = new LogFont();
            lf.lfCharSet = 1; // Default charset
            lf.lfFaceName = faceName;
            lf.lfHeight = height;
            lf.lfWeight = bold ? 700 : 0;
            lf.lfItalic = Convert.ToByte( italic );

            return new GdiFont( LibWrapper.CreateFontIndirect( lf ), faceName, height );
        }

        /// <summary>
        ///     Creates a font whose height is equal to the negative value
        ///     of the EM Square
        /// </summary>
        /// <param name="faceName">The typeface name of a font.</param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static GdiFont CreateDesignFont( string faceName, bool bold, bool italic, GdiDeviceContent dc )
        {
            // TODO: Is there a simpler method of obtaining the em-sqaure?
            GdiFont tempFont = CreateFont( faceName, 2048, bold, italic );
            dc.SelectFont( tempFont );
            GdiFontMetrics metrics = tempFont.GetMetrics( dc );
            tempFont.Dispose();

            return CreateFont( faceName, -Math.Abs( metrics.EmSquare ), bold, italic );
        }

        public GdiFontMetrics GetMetrics( GdiDeviceContent dc )
        {
            return new GdiFontMetrics( dc, this );
        }
    }
}