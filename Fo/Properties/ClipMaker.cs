using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ClipMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ClipMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ClipMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() ) );
        }
    }
}