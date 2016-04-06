namespace Fonet.Fo.Properties
{
    internal class MaxHeightMaker : LengthProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Constants.None );

        private Property _mDefaultProp;

        protected MaxHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MaxHeightMaker( propName );
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