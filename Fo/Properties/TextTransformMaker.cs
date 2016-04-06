using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TextTransformMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected TextTransformMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TextTransformMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}