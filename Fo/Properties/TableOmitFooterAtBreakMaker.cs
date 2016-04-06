namespace Fonet.Fo.Properties
{
    internal class TableOmitFooterAtBreakMaker : GenericBoolean
    {
        private Property m_defaultProp;

        protected TableOmitFooterAtBreakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TableOmitFooterAtBreakMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "false", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}