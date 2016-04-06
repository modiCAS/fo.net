using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class XMLLangMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected XMLLangMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new XMLLangMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}