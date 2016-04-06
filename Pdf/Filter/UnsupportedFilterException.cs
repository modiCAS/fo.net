using System;

namespace Fonet.Pdf.Filter
{
    public class UnsupportedFilterException : Exception
    {
        public UnsupportedFilterException( string filterName )
            : base( string.Format( "The {0} filter is not supported.", filterName ) )
        {
        }
    }
}