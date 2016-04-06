namespace Fonet.Fo.Properties
{
    internal class BorderCollapseMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropSeparate = new EnumProperty( Constants.Separate );

        protected static readonly EnumProperty SPropCollapse = new EnumProperty( Constants.Collapse );

        private Property _mDefaultProp;

        protected BorderCollapseMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new BorderCollapseMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "collapse", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "separate" ) )
                return SPropSeparate;

            if ( value.Equals( "collapse" ) )
                return SPropCollapse;

            return base.CheckEnumValues( value );
        }
    }
}