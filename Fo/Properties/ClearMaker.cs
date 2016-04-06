using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ClearMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ClearMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ClearMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() ) );
        }
    }
}