namespace Fonet.Fo.Properties
{
    internal class HyphenationPushCharacterCountMaker : NumberProperty.Maker
    {
        private Property m_defaultProp;

        protected HyphenationPushCharacterCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationPushCharacterCountMaker( propName );
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