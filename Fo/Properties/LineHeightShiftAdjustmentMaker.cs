using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineHeightShiftAdjustmentMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected LineHeightShiftAdjustmentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LineHeightShiftAdjustmentMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "consider-shifts", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}