namespace Fonet.DataTypes
{
    internal class Number
    {
        private readonly decimal _value;

        public Number( int n )
        {
            _value = n;
        }

        public Number( decimal n )
        {
            _value = n;
        }

        public Number( double n )
        {
            _value = (decimal)n;
        }

        public int IntValue()
        {
            return (int)_value;
        }

        public double DoubleValue()
        {
            return (double)_value;
        }

        public float FloatValue()
        {
            return (float)_value;
        }

        public decimal DecimalValue()
        {
            return _value;
        }
    }
}