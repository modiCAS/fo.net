using System;
using Fonet.DataTypes;

namespace Fonet.Layout
{
    internal class BorderAndPadding : ICloneable
    {
        public const int Top = 0;
        public const int Right = 1;
        public const int Bottom = 2;
        public const int Left = 3;

        private BorderInfo[] _borderInfo = new BorderInfo[ 4 ];
        private ResolvedCondLength[] _padding = new ResolvedCondLength[ 4 ];

        public object Clone()
        {
            var bp = new BorderAndPadding
            {
                _padding = (ResolvedCondLength[])_padding.Clone(),
                _borderInfo = (BorderInfo[])_borderInfo.Clone()
            };

            for ( var i = 0; i < _padding.Length; i++ )
            {
                if ( _padding[ i ] != null )
                    bp._padding[ i ] = (ResolvedCondLength)_padding[ i ].Clone();
                if ( _borderInfo[ i ] != null )
                    bp._borderInfo[ i ] = (BorderInfo)_borderInfo[ i ].Clone();
            }
            return bp;
        }

        public void SetBorder( int side, int style, CondLength width,
            ColorType color )
        {
            _borderInfo[ side ] = new BorderInfo( style, width, color );
        }

        public void SetPadding( int side, CondLength width )
        {
            _padding[ side ] = new ResolvedCondLength( width );
        }

        public void SetPaddingLength( int side, int iLength )
        {
            _padding[ side ].Length = iLength;
        }

        public void SetBorderLength( int side, int iLength )
        {
            _borderInfo[ side ].MWidth.Length = iLength;
        }

        public int GetBorderLeftWidth( bool bDiscard )
        {
            return GetBorderWidth( Left, bDiscard );
        }

        public int GetBorderRightWidth( bool bDiscard )
        {
            return GetBorderWidth( Right, bDiscard );
        }

        public int GetBorderTopWidth( bool bDiscard )
        {
            return GetBorderWidth( Top, bDiscard );
        }

        public int GetBorderBottomWidth( bool bDiscard )
        {
            return GetBorderWidth( Bottom, bDiscard );
        }

        public int GetPaddingLeft( bool bDiscard )
        {
            return GetPadding( Left, bDiscard );
        }

        public int GetPaddingRight( bool bDiscard )
        {
            return GetPadding( Right, bDiscard );
        }

        public int GetPaddingBottom( bool bDiscard )
        {
            return GetPadding( Bottom, bDiscard );
        }

        public int GetPaddingTop( bool bDiscard )
        {
            return GetPadding( Top, bDiscard );
        }


        private int GetBorderWidth( int side, bool bDiscard )
        {
            if ( _borderInfo[ side ] == null
                || bDiscard && _borderInfo[ side ].MWidth.BDiscard )
                return 0;
            return _borderInfo[ side ].MWidth.Length;
        }

        public ColorType GetBorderColor( int side )
        {
            return _borderInfo[ side ] != null ? _borderInfo[ side ].MColor : null;
        }

        public int GetBorderStyle( int side )
        {
            return _borderInfo[ side ] != null ? _borderInfo[ side ].MStyle : 0;
        }

        private int GetPadding( int side, bool bDiscard )
        {
            if ( _padding[ side ] == null || bDiscard && _padding[ side ].BDiscard )
                return 0;
            return _padding[ side ].Length;
        }

        private sealed class ResolvedCondLength : ICloneable
        {
            internal readonly bool BDiscard;
            internal int Length;

            private ResolvedCondLength( int iLength, bool bDiscard )
            {
                this.Length = iLength;
                this.BDiscard = bDiscard;
            }

            internal ResolvedCondLength( CondLength length )
            {
                BDiscard = length.IsDiscard();
                Length = length.MValue();
            }

            public object Clone()
            {
                return new ResolvedCondLength( Length, BDiscard );
            }
        }

        private sealed class BorderInfo : ICloneable
        {
            internal readonly ColorType MColor;
            internal readonly int MStyle;
            internal readonly ResolvedCondLength MWidth;

            internal BorderInfo( int style, CondLength width, ColorType color )
            {
                MStyle = style;
                MWidth = new ResolvedCondLength( width );
                MColor = color;
            }

            private BorderInfo( int style, ResolvedCondLength width, ColorType color )
            {
                MStyle = style;
                MWidth = width;
                MColor = color;
            }

            public object Clone()
            {
                return new BorderInfo(
                    MStyle, (ResolvedCondLength)MWidth.Clone(), (ColorType)MColor.Clone() );
            }
        }
    }
}