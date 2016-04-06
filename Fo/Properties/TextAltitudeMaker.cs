using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TextAltitudeMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected TextAltitudeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TextAltitudeMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "use-font-metrics", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}