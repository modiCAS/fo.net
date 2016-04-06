namespace Fonet.Fo.Properties
{
    internal class RelativeAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropBefore = new EnumProperty( Constants.Before );

        protected static readonly EnumProperty SPropBaseline = new EnumProperty( Constants.Baseline );

        private Property _mDefaultProp;

        protected RelativeAlignMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new RelativeAlignMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "before" ) )
                return SPropBefore;

            if ( value.Equals( "after" ) )
                return SPropBaseline;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "before", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}