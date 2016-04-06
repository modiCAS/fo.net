using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SwitchToMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SwitchToMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SwitchToMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "xsl-any", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}