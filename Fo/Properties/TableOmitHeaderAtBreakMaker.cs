namespace Fonet.Fo.Properties
{
    internal class TableOmitHeaderAtBreakMaker : GenericBoolean
    {
        private Property m_defaultProp;

        protected TableOmitHeaderAtBreakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TableOmitHeaderAtBreakMaker( propName );
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