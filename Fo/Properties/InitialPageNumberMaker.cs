namespace Fonet.Fo.Properties
{
    internal class InitialPageNumberMaker : StringProperty.Maker
    {
        private Property m_defaultProp;

        protected InitialPageNumberMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new InitialPageNumberMaker( propName );
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
    }
}