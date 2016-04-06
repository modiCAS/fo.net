using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSizeAdjustMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected FontSizeAdjustMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontSizeAdjustMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}