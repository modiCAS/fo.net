using System;
using System.Text;

namespace Fonet.Fo.Pagination
{
    internal class PageNumberGenerator
    {
        private const int Decimal = 1; // '0*1'
        private const int Loweralpha = 2; // 'a'
        private const int Upperalpha = 3; // 'A'
        private const int Lowerroman = 4; // 'i'
        private const int Upperroman = 5; // 'I'
        private string _format;

        private readonly int _formatType = Decimal;
        private char _groupingSeparator;
        private int _groupingSize;
        private int _letterValue;
        private readonly int _minPadding;

        private readonly string[] _zeros =
        {
            "", "0", "00", "000", "0000", "00000"
        };

        public PageNumberGenerator( string format, char groupingSeparator,
            int groupingSize, int letterValue )
        {
            this._format = format;
            this._groupingSeparator = groupingSeparator;
            this._groupingSize = groupingSize;
            this._letterValue = letterValue;

            int fmtLen = format.Length;
            if ( fmtLen == 1 )
            {
                if ( format.Equals( "1" ) )
                {
                    _formatType = Decimal;
                    _minPadding = 0;
                }
                else if ( format.Equals( "a" ) )
                    _formatType = Loweralpha;
                else if ( format.Equals( "A" ) )
                    _formatType = Upperalpha;
                else if ( format.Equals( "i" ) )
                    _formatType = Lowerroman;
                else if ( format.Equals( "I" ) )
                    _formatType = Upperroman;
                else
                {
                    _formatType = Decimal;
                    _minPadding = 0;
                }
            }
            else
            {
                for ( var i = 0; i < fmtLen - 1; i++ )
                {
                    if ( format[ i ] != '0' )
                    {
                        _formatType = Decimal;
                        _minPadding = 0;
                    }
                    else
                        _minPadding = fmtLen - 1;
                }
            }
        }

        public string MakeFormattedPageNumber( int number )
        {
            string pn = null;
            if ( _formatType == Decimal )
            {
                pn = number.ToString();
                if ( _minPadding >= pn.Length )
                {
                    int nz = _minPadding - pn.Length + 1;
                    pn = _zeros[ nz ] + pn;
                }
            }
            else if ( _formatType == Lowerroman || _formatType == Upperroman )
            {
                pn = MakeRoman( number );
                if ( _formatType == Upperroman )
                    pn = pn.ToUpper();
            }
            else
            {
                pn = MakeAlpha( number );
                if ( _formatType == Upperalpha )
                    pn = pn.ToUpper();
            }
            return pn;
        }

        private string MakeRoman( int num )
        {
            int[] arabic =
            {
                1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1
            };
            string[] roman =
            {
                "m", "cm", "d", "cd", "c", "xc", "l", "xl", "x", "ix", "v", "iv",
                "i"
            };

            var i = 0;
            var romanNumber = new StringBuilder();

            while ( num > 0 )
            {
                while ( num >= arabic[ i ] )
                {
                    num = num - arabic[ i ];
                    romanNumber.Append( roman[ i ] );
                }
                i = i + 1;
            }
            return romanNumber.ToString();
        }

        private string MakeAlpha( int num )
        {
            var letters = "abcdefghijklmnopqrstuvwxyz";
            var alphaNumber = new StringBuilder();

            var nbase = 26;
            var rem = 0;
            num--;
            if ( num < nbase )
                alphaNumber.Append( letters[ num ] );
            else
            {
                while ( num >= nbase )
                {
                    rem = num % nbase;
                    alphaNumber.Append( letters[ rem ] );
                    num = num / nbase;
                }
                alphaNumber.Append( letters[ num - 1 ] );
            }
            char[] strArray = alphaNumber.ToString().ToCharArray();
            Array.Reverse( strArray );
            return new string( strArray );
        }
    }
}