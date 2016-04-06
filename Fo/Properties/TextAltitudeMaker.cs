using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TextAltitudeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected TextAltitudeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TextAltitudeMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "use-font-metrics", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}