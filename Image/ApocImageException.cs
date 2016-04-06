using System;

namespace Fonet.Image
{
    internal class FonetImageException : Exception
    {
        public FonetImageException()
        {
        }

        public FonetImageException( string message ) : base( message )
        {
        }
    }
}