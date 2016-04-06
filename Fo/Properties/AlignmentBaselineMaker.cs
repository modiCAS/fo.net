using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AlignmentBaselineMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected AlignmentBaselineMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AlignmentBaselineMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}