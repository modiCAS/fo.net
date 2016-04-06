namespace Fonet.Fo.Properties
{
    internal class RetrieveBoundaryMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropPage = new EnumProperty( Constants.Page );

        protected static readonly EnumProperty SPropPageSequence = new EnumProperty( Constants.PageSequence );

        protected static readonly EnumProperty SPropDocument = new EnumProperty( Constants.Document );

        private Property _mDefaultProp;

        protected RetrieveBoundaryMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new RetrieveBoundaryMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "page" ) )
                return SPropPage;

            if ( value.Equals( "page-sequence" ) )
                return SPropPageSequence;

            if ( value.Equals( "document" ) )
                return SPropDocument;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "page-sequence", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}