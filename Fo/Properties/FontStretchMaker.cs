using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontStretchMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontStretchMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontStretchMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "normal", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}