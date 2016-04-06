using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class HyphenationLadderCountMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected HyphenationLadderCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationLadderCountMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "no-limit", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}