using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ContentTypeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ContentTypeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ContentTypeMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}