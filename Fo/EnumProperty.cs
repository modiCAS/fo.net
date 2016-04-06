namespace Fonet.Fo
{
    internal class EnumProperty : Property
    {
        private readonly int _value;

        public EnumProperty( int explicitValue )
        {
            _value = explicitValue;
        }

        public override int GetEnum()
        {
            return _value;
        }

        public override object GetObject()
        {
            return _value;
        }

        internal class Maker : PropertyMaker
        {
            protected Maker( string propName ) : base( propName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "Unknown enumerated value for property '"
                        + PropName + "': " + value );
                return null;
            }

            protected Property FindConstant( string value )
            {
                return null;
            }

            public override Property ConvertProperty( Property p,
                PropertyList propertyList,
                FObj fo )
            {
                if ( p is EnumProperty )
                    return p;
                return null;
            }
        }
    }
}