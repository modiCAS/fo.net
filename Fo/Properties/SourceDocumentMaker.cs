namespace Fonet.Fo.Properties
{
    internal class SourceDocumentMaker : StringProperty.Maker
    {
        private Property m_defaultProp;

        protected SourceDocumentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SourceDocumentMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}