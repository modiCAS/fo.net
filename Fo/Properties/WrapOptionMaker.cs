namespace Fonet.Fo.Properties
{
    internal class WrapOptionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropWrap = new EnumProperty( Constants.Wrap );

        protected static readonly EnumProperty SPropNoWrap = new EnumProperty( Constants.NoWrap );

        private Property _mDefaultProp;

        protected WrapOptionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WrapOptionMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "wrap" ) )
                return SPropWrap;

            if ( value.Equals( "no-wrap" ) )
                return SPropNoWrap;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "wrap", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}