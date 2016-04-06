namespace Fonet.Fo
{
    internal class EnumProperty : Property
    {
        private readonly int value;

        public EnumProperty( int explicitValue )
        {
            value = explicitValue;
        }

        public override int GetEnum()
        {
            return value;
        }

        public override object GetObject()
        {
            return value;
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

            protected Property findConstant( string value )
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