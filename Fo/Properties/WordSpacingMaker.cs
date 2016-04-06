using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class WordSpacingMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected WordSpacingMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WordSpacingMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "normal", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}