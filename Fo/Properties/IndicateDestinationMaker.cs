using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class IndicateDestinationMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected IndicateDestinationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new IndicateDestinationMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "false", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}