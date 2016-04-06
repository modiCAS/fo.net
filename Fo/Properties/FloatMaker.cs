using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FloatMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected FloatMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FloatMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}