using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundPositionHorizontalMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected BackgroundPositionHorizontalMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundPositionHorizontalMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "0%", propertyList.GetParentFObj() ) );
        }
    }
}