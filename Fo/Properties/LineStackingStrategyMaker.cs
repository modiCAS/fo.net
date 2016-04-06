using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineStackingStrategyMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected LineStackingStrategyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LineStackingStrategyMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "line-height", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}