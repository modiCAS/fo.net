namespace Fonet.Fo.Properties
{
    internal class FontVariantMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropNormal = new EnumProperty( Constants.Normal );

        protected static readonly EnumProperty SPropSmallCaps = new EnumProperty( Constants.SmallCaps );

        private Property _mDefaultProp;

        protected FontVariantMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new FontVariantMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "normal" ) )
                return SPropNormal;

            if ( value.Equals( "small-caps" ) )
                return SPropSmallCaps;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "normal", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}