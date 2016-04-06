using System;

namespace Fonet.Pdf.Gdi
{
    /// <summary>
    ///     A very lightweight wrapper around a Win32 device context
    /// </summary>
    public class GdiDeviceContent : IDisposable
    {
        /// <summary>
        ///     Pointer to device context created by ::CreateDC()
        /// </summary>
        private IntPtr _hDc;

        /// <summary>
        ///     Creates a new device context that matches the desktop display surface
        /// </summary>
        public GdiDeviceContent()
        {
            //this.hDC = LibWrapper.CreateDC("Display", String.Empty, null, IntPtr.Zero);
            _hDc = LibWrapper.GetDC( IntPtr.Zero );
        }

        /// <summary>
        ///     Returns a handle to the underlying device context
        /// </summary>
        internal IntPtr Handle
        {
            get { return _hDc; }
        }

        public virtual void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        ///     Invokes <see cref="Dispose(bool)" />.
        /// </summary>
        ~GdiDeviceContent()
        {
            Dispose( false );
        }

        /// <summary>
        ///     Delete the device context freeing the associated memory.
        /// </summary>
        protected virtual void Dispose( bool disposing )
        {
            if ( _hDc != IntPtr.Zero )
            {
                LibWrapper.DeleteDC( _hDc );

                // Mark as deleted
                _hDc = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Selects a font into a device context (DC). The new object
        ///     replaces the previous object of the same type.
        /// </summary>
        /// <param name="font">Handle to object.</param>
        /// <returns>A handle to the object being replaced.</returns>
        public IntPtr SelectFont( GdiFont font )
        {
            return LibWrapper.SelectObject( _hDc, font.Handle );
        }

        /// <summary>
        ///     Gets a handle to an object of the specified type that has been
        ///     selected into this device context.
        /// </summary>
        public IntPtr GetCurrentObject( GdiDcObject objectType )
        {
            return LibWrapper.GetCurrentObject( _hDc, objectType );
        }
    }
}