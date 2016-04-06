namespace Fonet.Fo.Properties
{
    internal class RetrieveBoundaryMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propPAGE = new EnumProperty( Constants.PAGE );

        protected static readonly EnumProperty s_propPAGE_SEQUENCE = new EnumProperty( Constants.PAGE_SEQUENCE );

        protected static readonly EnumProperty s_propDOCUMENT = new EnumProperty( Constants.DOCUMENT );

        private Property m_defaultProp;

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
                return s_propPAGE;

            if ( value.Equals( "page-sequence" ) )
                return s_propPAGE_SEQUENCE;

            if ( value.Equals( "document" ) )
                return s_propDOCUMENT;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "page-sequence", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}