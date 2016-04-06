using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class XmlLangMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected XmlLangMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new XmlLangMaker( propName );
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