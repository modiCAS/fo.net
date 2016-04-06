namespace Fonet.Fo.Properties
{
    internal class TableLayoutMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propAUTO = new EnumProperty( Constants.AUTO );

        protected static readonly EnumProperty s_propFIXED = new EnumProperty( Constants.FIXED );

        private Property m_defaultProp;

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
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "auto" ) )
                return s_propAUTO;

            if ( value.Equals( "fixed" ) )
                return s_propFIXED;

            return base.CheckEnumValues( value );
        }
    }
}