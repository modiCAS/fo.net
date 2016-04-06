namespace Fonet.Fo.Properties
{
    internal class BorderCollapseMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propSEPARATE = new EnumProperty( Constants.SEPARATE );

        protected static readonly EnumProperty s_propCOLLAPSE = new EnumProperty( Constants.COLLAPSE );

        private Property m_defaultProp;

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
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "collapse", propertyList.getParentFObj() );
            return m_defaultProp;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "separate" ) )
                return s_propSEPARATE;

            if ( value.Equals( "collapse" ) )
                return s_propCOLLAPSE;

            return base.CheckEnumValues( value );
        }
    }
}