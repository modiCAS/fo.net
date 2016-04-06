namespace Fonet.Fo.Properties
{
    internal class PositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propSTATIC = new EnumProperty( Constants.STATIC );

        protected static readonly EnumProperty s_propRELATIVE = new EnumProperty( Constants.RELATIVE );

        protected static readonly EnumProperty s_propABSOLUTE = new EnumProperty( Constants.ABSOLUTE );

        protected static readonly EnumProperty s_propFIXED = new EnumProperty( Constants.FIXED );

        private Property m_defaultProp;

        protected PositionMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new PositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "static" ) )
                return s_propSTATIC;

            if ( value.Equals( "relative" ) )
                return s_propRELATIVE;

            if ( value.Equals( "absolute" ) )
                return s_propABSOLUTE;

            if ( value.Equals( "fixed" ) )
                return s_propFIXED;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "static", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}