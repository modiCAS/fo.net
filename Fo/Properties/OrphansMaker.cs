namespace Fonet.Fo.Properties
{
    internal class OrphansMaker : NumberProperty.Maker
    {
        private Property m_defaultProp;

        protected OrphansMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new OrphansMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "2", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}