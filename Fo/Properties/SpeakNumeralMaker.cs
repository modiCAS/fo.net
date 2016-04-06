using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakNumeralMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SpeakNumeralMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakNumeralMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "continuous", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}