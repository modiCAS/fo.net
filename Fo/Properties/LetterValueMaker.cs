namespace Fonet.Fo.Properties
{
    internal class LetterValueMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropAlphabetic = new EnumProperty( Constants.Alphabetic );

        protected static readonly EnumProperty SPropTraditional = new EnumProperty( Constants.Traditional );

        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        private Property _mDefaultProp;

        protected LetterValueMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new LetterValueMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "alphabetic" ) )
                return SPropAlphabetic;

            if ( value.Equals( "traditional" ) )
                return SPropTraditional;

            if ( value.Equals( "auto" ) )
                return SPropAuto;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}