using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class NumericProperty : Property
    {
        private readonly Numeric numeric;

        internal NumericProperty( Numeric value )
        {
            numeric = value;
        }

        public override Numeric GetNumeric()
        {
            return numeric;
        }

        public override Number GetNumber()
        {
            return numeric.asNumber();
        }

        public override Length GetLength()
        {
            return numeric.asLength();
        }

        public override ColorType GetColorType()
        {
            return null;
        }

        public override object GetObject()
        {
            return numeric;
        }
    }
}