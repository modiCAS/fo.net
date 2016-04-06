namespace Fonet.Fo.Properties
{
    internal class GenericBoolean : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropTrue = new EnumProperty( Enums.True );

        protected static readonly EnumProperty SPropFalse = new EnumProperty( Enums.False );

        protected GenericBoolean( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericBoolean( propName );
        }


        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "true" ) )
                return SPropTrue;

            if ( value.Equals( "false" ) )
                return SPropFalse;

            return base.CheckEnumValues( value );
        }

        internal class Enums
        {
            public const int True = Constants.True;

            public const int False = Constants.False;
        }
    }
}