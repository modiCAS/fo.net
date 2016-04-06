using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AzimuthMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected AzimuthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AzimuthMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "center", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}