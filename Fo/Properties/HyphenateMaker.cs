namespace Fonet.Fo.Properties
{
    internal class HyphenateMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propTRUE = new EnumProperty( Constants.TRUE );

        protected static readonly EnumProperty s_propFALSE = new EnumProperty( Constants.FALSE );

        private Property m_defaultProp;

        protected HyphenateMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenateMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "true" ) )
                return s_propTRUE;

            if ( value.Equals( "false" ) )
                return s_propFALSE;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "false", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}