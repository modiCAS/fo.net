namespace Fonet.Fo.Properties
{
    internal class KeepWithPreviousMaker : GenericKeep
    {
        private Property m_defaultProp;

        protected KeepWithPreviousMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new KeepWithPreviousMaker( propName );
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