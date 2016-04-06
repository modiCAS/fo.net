using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class WhiteSpaceTreatmentMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected WhiteSpaceTreatmentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WhiteSpaceTreatmentMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "preserve", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}