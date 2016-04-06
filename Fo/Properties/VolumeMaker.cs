using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class VolumeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected VolumeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new VolumeMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "medium", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}