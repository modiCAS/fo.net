using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LinefeedTreatmentMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected LinefeedTreatmentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LinefeedTreatmentMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "treat-as-space", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}