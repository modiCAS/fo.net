namespace Fonet.Fo.Properties
{
    internal class FormatMaker : StringProperty.Maker
    {
        private Property m_defaultProp;

        protected FormatMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FormatMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "1", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}