namespace Fonet.Fo.Properties
{
    internal class BaselineShiftMaker : LengthProperty.Maker
    {
        protected static readonly EnumProperty s_propBASELINE = new EnumProperty( Constants.BASELINE );

        protected static readonly EnumProperty s_propSUB = new EnumProperty( Constants.SUB );

        protected static readonly EnumProperty s_propSUPER = new EnumProperty( Constants.SUPER );

        private Property m_defaultProp;

        protected BaselineShiftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BaselineShiftMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "baseline" ) )
                return s_propBASELINE;

            if ( value.Equals( "sub" ) )
                return s_propSUB;

            if ( value.Equals( "super" ) )
                return s_propSUPER;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "baseline", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}