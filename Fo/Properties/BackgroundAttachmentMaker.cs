using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundAttachmentMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected BackgroundAttachmentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundAttachmentMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "scroll", propertyList.GetParentFObj() ) );
        }
    }
}