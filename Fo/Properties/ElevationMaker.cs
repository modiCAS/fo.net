using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ElevationMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ElevationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ElevationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "level", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}