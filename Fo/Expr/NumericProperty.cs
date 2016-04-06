using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class NumericProperty : Property
    {
        private readonly Numeric _numeric;

        internal NumericProperty( Numeric value )
        {
            _numeric = value;
        }

        public override Numeric GetNumeric()
        {
            return _numeric;
        }

        public override Number GetNumber()
        {
            return _numeric.AsNumber();
        }

        public override Length GetLength()
        {
            return _numeric.AsLength();
        }

        public override ColorType GetColorType()
        {
            return null;
        }

        public override object GetObject()
        {
            return _numeric;
        }
    }
}