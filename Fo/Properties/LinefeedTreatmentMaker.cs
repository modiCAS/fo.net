using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LinefeedTreatmentMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected LinefeedTreatmentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LinefeedTreatmentMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "treat-as-space", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}