using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AzimuthMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected AzimuthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AzimuthMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "center", propertyList.GetParentFObj() ) );
        }
    }
}