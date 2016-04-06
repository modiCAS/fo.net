using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ColorProfileNameMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ColorProfileNameMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColorProfileNameMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}