using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSelectionStrategyMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected FontSelectionStrategyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontSelectionStrategyMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}