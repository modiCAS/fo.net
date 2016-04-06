using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineStackingStrategyMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

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
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "line-height", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}