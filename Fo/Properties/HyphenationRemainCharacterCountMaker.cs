namespace Fonet.Fo.Properties
{
    internal class HyphenationRemainCharacterCountMaker : NumberProperty.Maker
    {
        private Property m_defaultProp;

        protected HyphenationRemainCharacterCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationRemainCharacterCountMaker( propName );
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