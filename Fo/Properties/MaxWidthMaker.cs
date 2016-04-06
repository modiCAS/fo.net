namespace Fonet.Fo.Properties
{
    internal class MaxWidthMaker : LengthProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Constants.None );

        private Property _mDefaultProp;

        protected MaxWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MaxWidthMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "none" ) )
                return SPropNone;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}