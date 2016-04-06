using System;
using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class Numeric
    {
        public const int AbsLength = 1;
        public const int PcLength = 2;
        public const int TcolLength = 4;
        private readonly double _absValue;
        private readonly int _dim;
        private readonly IPercentBase _pcBase;
        private readonly double _pcValue;
        private readonly double _tcolValue;

        private readonly int _valType;

        protected Numeric( int valType, double absValue, double pcValue,
            double tcolValue, int dim, IPercentBase pcBase )
        {
            this._valType = valType;
            this._absValue = absValue;
            this._pcValue = pcValue;
            this._tcolValue = tcolValue;
            this._dim = dim;
            this._pcBase = pcBase;
        }

        public Numeric( decimal num ) :
            this( AbsLength, (double)num, 0.0, 0.0, 0, null )
        {
        }

        public Numeric( FixedLength l ) :
            this( AbsLength, l.MValue(), 0.0, 0.0, 1, null )
        {
        }

        public Numeric( PercentLength pclen ) :
            this( PcLength, 0.0, pclen.Value(), 0.0, 1, pclen.BaseLength )
        {
        }

        public Numeric( TableColLength tclen ) :
            this( TcolLength, 0.0, 0.0, tclen.GetTableUnits(), 1, null )
        {
        }

        public Length AsLength()
        {
            if ( _dim == 1 )
            {
                var len = new ArrayList( 3 );
                if ( ( _valType & AbsLength ) != 0 )
                    len.Add( new FixedLength( (int)_absValue ) );
                if ( ( _valType & PcLength ) != 0 )
                    len.Add( new PercentLength( _pcValue, _pcBase ) );
                if ( ( _valType & TcolLength ) != 0 )
                    len.Add( new TableColLength( _tcolValue ) );
                if ( len.Count == 1 )
                    return (Length)len[ 0 ];
                return new MixedLength( len );
            }
            return null;
        }

        public Number AsNumber()
        {
            return new Number( AsDouble() );
        }

        public double AsDouble()
        {
            if ( _dim == 0 && _valType == AbsLength )
                return _absValue;
            throw new Exception( "cannot make number if dimension != 0" );
        }

        private bool IsMixedType()
        {
            var ntype = 0;
            for ( int t = _valType; t != 0; t = t >> 1 )
            {
                if ( ( t & 1 ) != 0 )
                    ++ntype;
            }
            return ntype > 1;
        }

        public Numeric Subtract( Numeric op )
        {
            if ( _dim == op._dim )
            {
                IPercentBase npcBase = ( _valType & PcLength ) != 0
                    ? _pcBase
                    : op._pcBase;
                return new Numeric( _valType | op._valType, _absValue - op._absValue,
                    _pcValue - op._pcValue,
                    _tcolValue - op._tcolValue, _dim, npcBase );
            }
            throw new PropertyException( "Can't add Numerics of different dimensions" );
        }

        public Numeric Add( Numeric op )
        {
            if ( _dim == op._dim )
            {
                IPercentBase npcBase = ( _valType & PcLength ) != 0
                    ? _pcBase
                    : op._pcBase;
                return new Numeric( _valType | op._valType, _absValue + op._absValue,
                    _pcValue + op._pcValue,
                    _tcolValue + op._tcolValue, _dim, npcBase );
            }
            throw new PropertyException( "Can't add Numerics of different dimensions" );
        }

        public Numeric Multiply( Numeric op )
        {
            if ( _dim == 0 )
            {
                return new Numeric( op._valType, _absValue * op._absValue,
                    _absValue * op._pcValue,
                    _absValue * op._tcolValue, op._dim, op._pcBase );
            }
            if ( op._dim == 0 )
            {
                double opval = op._absValue;
                return new Numeric( _valType, opval * _absValue, opval * _pcValue,
                    opval * _tcolValue, _dim, _pcBase );
            }
            if ( _valType == op._valType && !IsMixedType() )
            {
                IPercentBase npcBase = ( _valType & PcLength ) != 0
                    ? _pcBase
                    : op._pcBase;
                return new Numeric( _valType, _absValue * op._absValue,
                    _pcValue * op._pcValue,
                    _tcolValue * op._tcolValue, _dim + op._dim,
                    npcBase );
            }
            throw new PropertyException( "Can't multiply mixed Numerics" );
        }

        public Numeric Divide( Numeric op )
        {
            if ( _dim == 0 )
            {
                return new Numeric( op._valType, _absValue / op._absValue,
                    _absValue / op._pcValue,
                    _absValue / op._tcolValue, -op._dim, op._pcBase );
            }
            if ( op._dim == 0 )
            {
                double opval = op._absValue;
                return new Numeric( _valType, _absValue / opval, _pcValue / opval,
                    _tcolValue / opval, _dim, _pcBase );
            }
            if ( _valType == op._valType && !IsMixedType() )
            {
                IPercentBase npcBase = ( _valType & PcLength ) != 0
                    ? _pcBase
                    : op._pcBase;
                return new Numeric( _valType,
                    _valType == AbsLength ? _absValue / op._absValue : 0.0,
                    _valType == PcLength ? _pcValue / op._pcValue : 0.0,
                    _valType == TcolLength ? _tcolValue / op._tcolValue : 0.0,
                    _dim - op._dim, npcBase );
            }
            throw new PropertyException( "Can't divide mixed Numerics." );
        }

        public Numeric Abs()
        {
            return new Numeric( _valType, Math.Abs( _absValue ), Math.Abs( _pcValue ),
                Math.Abs( _tcolValue ), _dim, _pcBase );
        }

        public Numeric Max( Numeric op )
        {
            var rslt = 0.0;
            if ( _dim == op._dim && _valType == op._valType && !IsMixedType() )
            {
                if ( _valType == AbsLength )
                    rslt = _absValue - op._absValue;
                else if ( _valType == PcLength )
                    rslt = _pcValue - op._pcValue;
                else if ( _valType == TcolLength )
                    rslt = _tcolValue - op._tcolValue;
                if ( rslt > 0.0 )
                    return this;
                return op;
            }
            throw new PropertyException( "Arguments to max() must have same dimension and value type." );
        }

        public Numeric Min( Numeric op )
        {
            var rslt = 0.0;
            if ( _dim == op._dim && _valType == op._valType && !IsMixedType() )
            {
                if ( _valType == AbsLength )
                    rslt = _absValue - op._absValue;
                else if ( _valType == PcLength )
                    rslt = _pcValue - op._pcValue;
                else if ( _valType == TcolLength )
                    rslt = _tcolValue - op._tcolValue;
                if ( rslt > 0.0 )
                    return op;
                return this;
            }
            throw new PropertyException( "Arguments to min() must have same dimension and value type." );
        }
    }
}