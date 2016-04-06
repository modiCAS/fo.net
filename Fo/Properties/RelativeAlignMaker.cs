namespace Fonet.Fo.Properties
{
    internal class RelativeAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propBEFORE = new EnumProperty( Constants.BEFORE );

        protected static readonly EnumProperty s_propBASELINE = new EnumProperty( Constants.BASELINE );

        private Property m_defaultProp;

        protected RelativeAlignMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new RelativeAlignMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "before" ) )
                return s_propBEFORE;

            if ( value.Equals( "after" ) )
                return s_propBASELINE;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "before", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}