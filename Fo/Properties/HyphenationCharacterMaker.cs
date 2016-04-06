namespace Fonet.Fo.Properties
{
    internal class HyphenationCharacterMaker : CharacterProperty.Maker
    {
        private Property m_defaultProp;

        protected HyphenationCharacterMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationCharacterMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "-", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}