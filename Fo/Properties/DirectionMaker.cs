using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class DirectionMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected DirectionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new DirectionMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "ltr", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}