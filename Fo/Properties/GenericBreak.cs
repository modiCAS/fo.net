namespace Fonet.Fo.Properties
{
    internal class GenericBreak : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropAuto = new EnumProperty( Enums.Auto );

        protected static readonly EnumProperty SPropColumn = new EnumProperty( Enums.Column );

        protected static readonly EnumProperty SPropPage = new EnumProperty( Enums.Page );

        protected static readonly EnumProperty SPropEvenPage = new EnumProperty( Enums.EvenPage );

        protected static readonly EnumProperty SPropOddPage = new EnumProperty( Enums.OddPage );

        private Property _mDefaultProp;

        protected GenericBreak( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericBreak( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "auto" ) )
                return SPropAuto;

            if ( value.Equals( "column" ) )
                return SPropColumn;

            if ( value.Equals( "page" ) )
                return SPropPage;

            if ( value.Equals( "even-page" ) )
                return SPropEvenPage;

            if ( value.Equals( "odd-page" ) )
                return SPropOddPage;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        internal class Enums
        {
            public const int Auto = Constants.Auto;

            public const int Column = Constants.Column;

            public const int Page = Constants.Page;

            public const int EvenPage = Constants.EvenPage;

            public const int OddPage = Constants.OddPage;
        }
    }
}