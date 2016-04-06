using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineHeightShiftAdjustmentMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

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
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "consider-shifts", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}