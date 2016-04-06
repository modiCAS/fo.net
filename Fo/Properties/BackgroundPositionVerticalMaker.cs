using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundPositionVerticalMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected BackgroundPositionVerticalMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundPositionVerticalMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0%", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}