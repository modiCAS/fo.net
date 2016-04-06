using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundPositionMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected BackgroundPositionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundPositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0%", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}