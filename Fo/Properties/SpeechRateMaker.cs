using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeechRateMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SpeechRateMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeechRateMaker( propName );
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