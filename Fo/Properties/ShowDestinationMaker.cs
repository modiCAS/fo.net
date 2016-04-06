using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ShowDestinationMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ShowDestinationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ShowDestinationMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "replace", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}