namespace Fonet.Fo.Properties
{
    internal class TableLayoutMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        protected static readonly EnumProperty SPropFixed = new EnumProperty( Constants.Fixed );

        private Property _mDefaultProp;

        protected TableLayoutMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TableLayoutMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "auto" ) )
                return SPropAuto;

            if ( value.Equals( "fixed" ) )
                return SPropFixed;

            return base.CheckEnumValues( value );
        }
    }
}