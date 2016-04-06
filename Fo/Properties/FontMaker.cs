using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected FontMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}