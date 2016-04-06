using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TextTransformMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected TextTransformMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TextTransformMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}