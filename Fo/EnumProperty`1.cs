namespace Fonet.Fo
{
    internal sealed class EnumProperty<T> : Property
    {
        private readonly T _value;

        public EnumProperty( T value )
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }
}