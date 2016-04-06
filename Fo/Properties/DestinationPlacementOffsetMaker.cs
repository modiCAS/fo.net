using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class DestinationPlacementOffsetMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected DestinationPlacementOffsetMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new DestinationPlacementOffsetMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}