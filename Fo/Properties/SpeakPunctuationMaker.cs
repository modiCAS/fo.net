using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakPunctuationMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SpeakPunctuationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakPunctuationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}