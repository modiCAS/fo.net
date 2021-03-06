using Fonet.DataTypes;
using Fonet.Fo.Expr;

namespace Fonet.Fo
{
    internal class NumberProperty : Property
    {
        private readonly decimal _number;

        public NumberProperty( Number num )
        {
            _number = num.DecimalValue();
        }

        public NumberProperty( decimal num )
        {
            _number = num;
        }

        public NumberProperty( double num )
        {
            _number = (decimal)num;
        }

        public NumberProperty( int num )
        {
            _number = num;
        }

        public override Number GetNumber()
        {
            return new Number( _number );
        }

        public override object GetObject()
        {
            return _number;
        }

        public override Numeric GetNumeric()
        {
            return new Numeric( _number );
        }

        public override ColorType GetColorType()
        {
            return new ColorType( (float)0.0, (float)0.0, (float)0.0 );
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string propName ) : base( propName )
            {
            }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo )
            {
                if ( p is NumberProperty )
                    return p;
                Number val = p.GetNumber();
                if ( val != null )
                    return new NumberProperty( val );
                return ConvertPropertyDatatype( p, propertyList, fo );
            }
        }
    }
}