using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LastLineEndIndentMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected LastLineEndIndentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LastLineEndIndentMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}