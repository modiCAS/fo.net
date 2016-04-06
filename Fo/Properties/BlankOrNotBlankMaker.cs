namespace Fonet.Fo.Properties
{
    internal class BlankOrNotBlankMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropBlank = new EnumProperty( Constants.Blank );

        protected static readonly EnumProperty SPropNotBlank = new EnumProperty( Constants.NotBlank );

        protected static readonly EnumProperty SPropAny = new EnumProperty( Constants.Any );

        private Property _mDefaultProp;

        protected BlankOrNotBlankMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BlankOrNotBlankMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "blank" ) )
                return SPropBlank;

            if ( value.Equals( "not-blank" ) )
                return SPropNotBlank;

            if ( value.Equals( "any" ) )
                return SPropAny;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "any", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}