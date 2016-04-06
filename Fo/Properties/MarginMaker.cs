using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class MarginMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected MarginMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MarginMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}