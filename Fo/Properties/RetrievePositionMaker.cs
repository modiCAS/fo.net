namespace Fonet.Fo.Properties
{
    internal class RetrievePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropFswp = new EnumProperty( Constants.Fswp );

        protected static readonly EnumProperty SPropFic = new EnumProperty( Constants.Fic );

        protected static readonly EnumProperty SPropLswp = new EnumProperty( Constants.Lswp );

        protected static readonly EnumProperty SPropLewp = new EnumProperty( Constants.Lewp );

        private Property _mDefaultProp;

        protected RetrievePositionMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new RetrievePositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "first-starting-within-page" ) )
                return SPropFswp;

            if ( value.Equals( "first-including-carryover" ) )
                return SPropFic;

            if ( value.Equals( "last-starting-within-page" ) )
                return SPropLswp;

            if ( value.Equals( "last-ending-within-page" ) )
                return SPropLewp;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "first-starting-within-page", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}