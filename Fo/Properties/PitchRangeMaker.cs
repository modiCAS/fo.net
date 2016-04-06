using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PitchRangeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PitchRangeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PitchRangeMaker( propName );
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