namespace Fonet.Fo.Properties
{
    internal class BaselineShiftMaker : LengthProperty.Maker
    {
        protected static readonly EnumProperty SPropBaseline = new EnumProperty( Constants.Baseline );

        protected static readonly EnumProperty SPropSub = new EnumProperty( Constants.Sub );

        protected static readonly EnumProperty SPropSuper = new EnumProperty( Constants.Super );

        private Property _mDefaultProp;

        protected BaselineShiftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BaselineShiftMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "baseline" ) )
                return SPropBaseline;

            if ( value.Equals( "sub" ) )
                return SPropSub;

            if ( value.Equals( "super" ) )
                return SPropSuper;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "baseline", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}