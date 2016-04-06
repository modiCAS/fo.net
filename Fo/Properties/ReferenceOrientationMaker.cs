using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ReferenceOrientationMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ReferenceOrientationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ReferenceOrientationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}