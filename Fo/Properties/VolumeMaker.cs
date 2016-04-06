using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class VolumeMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected VolumeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new VolumeMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "medium", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}