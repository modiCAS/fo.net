using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class VisibilityMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected VisibilityMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new VisibilityMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "visible", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}