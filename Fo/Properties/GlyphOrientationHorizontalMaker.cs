using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GlyphOrientationHorizontalMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected GlyphOrientationHorizontalMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GlyphOrientationHorizontalMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0deg", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}