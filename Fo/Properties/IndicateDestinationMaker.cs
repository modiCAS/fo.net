using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class IndicateDestinationMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected IndicateDestinationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new IndicateDestinationMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "false", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}