namespace Fonet.Fo.Properties
{
    internal class OddOrEvenMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropOdd = new EnumProperty( Constants.Odd );

        protected static readonly EnumProperty SPropEven = new EnumProperty( Constants.Even );

        protected static readonly EnumProperty SPropAny = new EnumProperty( Constants.Any );

        private Property _mDefaultProp;

        protected OddOrEvenMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new OddOrEvenMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "odd" ) )
                return SPropOdd;

            if ( value.Equals( "even" ) )
                return SPropEven;

            if ( value.Equals( "any" ) )
                return SPropAny;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "any", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}