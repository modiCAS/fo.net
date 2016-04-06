namespace Fonet.Fo.Properties
{
    internal class WhiteSpaceCollapseMaker : GenericBoolean
    {
        private Property m_defaultProp;

        protected WhiteSpaceCollapseMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WhiteSpaceCollapseMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "true", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}