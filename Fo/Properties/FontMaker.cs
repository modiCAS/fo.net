using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}