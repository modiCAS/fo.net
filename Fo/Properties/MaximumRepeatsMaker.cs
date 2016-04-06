namespace Fonet.Fo.Properties
{
    internal class MaximumRepeatsMaker : StringProperty.Maker
    {
        private Property m_defaultProp;

        protected MaximumRepeatsMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MaximumRepeatsMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "no-limit", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}