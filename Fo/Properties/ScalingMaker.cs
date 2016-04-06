namespace Fonet.Fo.Properties
{
    internal class ScalingMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropUniform = new EnumProperty( Constants.Uniform );

        protected static readonly EnumProperty SPropNonUniform = new EnumProperty( Constants.NonUniform );

        private Property _mDefaultProp;

        protected ScalingMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new ScalingMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "uniform" ) )
                return SPropUniform;

            if ( value.Equals( "non-uniform" ) )
                return SPropNonUniform;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "uniform", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}