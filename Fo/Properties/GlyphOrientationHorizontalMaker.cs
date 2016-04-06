using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GlyphOrientationHorizontalMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected GlyphOrientationHorizontalMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GlyphOrientationHorizontalMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0deg", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}