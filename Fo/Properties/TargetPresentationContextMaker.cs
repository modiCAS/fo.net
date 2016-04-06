using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TargetPresentationContextMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected TargetPresentationContextMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TargetPresentationContextMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "use-target-processing-context", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}