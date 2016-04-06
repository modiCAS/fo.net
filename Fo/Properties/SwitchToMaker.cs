using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SwitchToMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SwitchToMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SwitchToMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "xsl-any", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}