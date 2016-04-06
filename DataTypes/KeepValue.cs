namespace Fonet.DataTypes
{
    internal class KeepValue
    {
        public const string KeepWithAlways = "KEEP_WITH_ALWAYS";
        public const string KeepWithAuto = "KEEP_WITH_AUTO";
        public const string KeepWithValue = "KEEP_WITH_VALUE";

        private readonly string _type = KeepWithAuto;
        private readonly int _value;

        public KeepValue( string type, int val )
        {
            this._type = type;
            _value = val;
        }

        public int GetValue()
        {
            return _value;
        }

        public string GetKeepType()
        {
            return _type;
        }

        public override string ToString()
        {
            return _type;
        }
    }
}