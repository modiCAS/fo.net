using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontStretchMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected FontStretchMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontStretchMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "normal", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}