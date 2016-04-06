namespace Fonet.Fo.Properties
{
    internal class PagePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropFirst = new EnumProperty( Constants.First );

        protected static readonly EnumProperty SPropLast = new EnumProperty( Constants.Last );

        protected static readonly EnumProperty SPropRest = new EnumProperty( Constants.Rest );

        protected static readonly EnumProperty SPropAny = new EnumProperty( Constants.Any );

        private Property _mDefaultProp;

        protected PagePositionMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new PagePositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "first" ) )
                return SPropFirst;

            if ( value.Equals( "last" ) )
                return SPropLast;

            if ( value.Equals( "rest" ) )
                return SPropRest;

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