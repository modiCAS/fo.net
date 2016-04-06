using System;
using Fonet.DataTypes;

namespace Fonet.Layout
{
    internal class BorderAndPadding : ICloneable
    {
        public const int TOP = 0;
        public const int RIGHT = 1;
        public const int BOTTOM = 2;
        public const int LEFT = 3;

        private BorderInfo[] borderInfo = new BorderInfo[ 4 ];
        private ResolvedCondLength[] padding = new ResolvedCondLength[ 4 ];

        public object Clone()
        {
            var bp = new BorderAndPadding();
            bp.padding = (ResolvedCondLength[])padding.Clone();
            bp.borderInfo = (BorderInfo[])borderInfo.Clone();
            for ( var i = 0; i < padding.Length; i++ )
            {
                if ( padding[ i ] != null )
                    bp.padding[ i ] = (ResolvedCondLength)padding[ i ].Clone();
                if ( borderInfo[ i ] != null )
                    bp.borderInfo[ i ] = (BorderInfo)borderInfo[ i ].Clone();
            }
            return bp;
        }

        public void setBorder( int side, int style, CondLength width,
            ColorType color )
        {
            borderInfo[ side ] = new BorderInfo( style, width, color );
        }

        public void setPadding( int side, CondLength width )
        {
            padding[ side ] = new ResolvedCondLength( width );
        }

        public void setPaddingLength( int side, int iLength )
        {
            padding[ side ].iLength = iLength;
        }

        public void setBorderLength( int side, int iLength )
        {
            borderInfo[ side ].mWidth.iLength = iLength;
        }

        public int getBorderLeftWidth( bool bDiscard )
        {
            return getBorderWidth( LEFT, bDiscard );
        }

        public int getBorderRightWidth( bool bDiscard )
        {
            return getBorderWidth( RIGHT, bDiscard );
        }

        public int getBorderTopWidth( bool bDiscard )
        {
            return getBorderWidth( TOP, bDiscard );
        }

        public int getBorderBottomWidth( bool bDiscard )
        {
            return getBorderWidth( BOTTOM, bDiscard );
        }

        public int getPaddingLeft( bool bDiscard )
        {
            return getPadding( LEFT, bDiscard );
        }

        public int getPaddingRight( bool bDiscard )
        {
            return getPadding( RIGHT, bDiscard );
        }

        public int getPaddingBottom( bool bDiscard )
        {
            return getPadding( BOTTOM, bDiscard );
        }

        public int getPaddingTop( bool bDiscard )
        {
            return getPadding( TOP, bDiscard );
        }


        private int getBorderWidth( int side, bool bDiscard )
        {
            if ( borderInfo[ side ] == null
                || bDiscard && borderInfo[ side ].mWidth.bDiscard )
                return 0;
            return borderInfo[ side ].mWidth.iLength;
        }

        public ColorType getBorderColor( int side )
        {
            if ( borderInfo[ side ] != null )
                return borderInfo[ side ].mColor;
            return null;
        }

        public int getBorderStyle( int side )
        {
            if ( borderInfo[ side ] != null )
                return borderInfo[ side ].mStyle;
            return 0;
        }

        private int getPadding( int side, bool bDiscard )
        {
            if ( padding[ side ] == null || bDiscard && padding[ side ].bDiscard )
                return 0;
            return padding[ side ].iLength;
        }

        internal class ResolvedCondLength : ICloneable
        {
            internal bool bDiscard;
            internal int iLength;

            private ResolvedCondLength( int iLength, bool bDiscard )
            {
                this.iLength = iLength;
                this.bDiscard = bDiscard;
            }

            internal ResolvedCondLength( CondLength length )
            {
                bDiscard = length.IsDiscard();
                iLength = length.MValue();
            }

            public object Clone()
            {
                return new ResolvedCondLength( iLength, bDiscard );
            }
        }

        internal class BorderInfo : ICloneable
        {
            internal ColorType mColor;
            internal int mStyle;
            internal ResolvedCondLength mWidth;

            internal BorderInfo( int style, CondLength width, ColorType color )
            {
                mStyle = style;
                mWidth = new ResolvedCondLength( width );
                mColor = color;
            }

            private BorderInfo( int style, ResolvedCondLength width, ColorType color )
            {
                mStyle = style;
                mWidth = width;
                mColor = color;
            }

            public object Clone()
            {
                return new BorderInfo(
                    mStyle, (ResolvedCondLength)mWidth.Clone(), (ColorType)mColor.Clone() );
            }
        }
    }
}