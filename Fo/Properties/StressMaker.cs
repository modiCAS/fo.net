using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class StressMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected StressMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new StressMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "50", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}