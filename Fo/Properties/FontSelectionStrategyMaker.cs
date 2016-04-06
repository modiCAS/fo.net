using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSelectionStrategyMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

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
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}