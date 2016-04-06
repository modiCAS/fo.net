using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class UnicodeBidiMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected UnicodeBidiMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new UnicodeBidiMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "normal", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}