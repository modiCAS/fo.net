using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ElevationMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ElevationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ElevationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "level", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}