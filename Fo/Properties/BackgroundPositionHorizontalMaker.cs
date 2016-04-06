using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundPositionHorizontalMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected BackgroundPositionHorizontalMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundPositionHorizontalMaker( propName );
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