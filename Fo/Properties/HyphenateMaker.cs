namespace Fonet.Fo.Properties
{
    internal class HyphenateMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropTrue = new EnumProperty( Constants.True );

        protected static readonly EnumProperty SPropFalse = new EnumProperty( Constants.False );

        private Property _mDefaultProp;

        protected HyphenateMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenateMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "true" ) )
                return SPropTrue;

            if ( value.Equals( "false" ) )
                return SPropFalse;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "false", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}