using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class DestinationPlacementOffsetMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected DestinationPlacementOffsetMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new DestinationPlacementOffsetMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}