using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TargetProcessingContextMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected TargetProcessingContextMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TargetProcessingContextMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "document-root", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}