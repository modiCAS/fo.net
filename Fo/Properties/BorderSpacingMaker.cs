using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BorderSpacingMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected BorderSpacingMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderSpacingMaker( propName );
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